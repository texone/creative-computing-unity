using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;

public class RDMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
    /// Any subsequent call within the same solution will increment the Iteration count.
    /// </summary>
    private readonly int Iteration;


    /// <summary>
    /// This procedure contains the user code. Input parameters are provided as regular arguments,
    /// Output parameters as ref arguments. You don't have to assign output parameters,
    /// they will have a default value.
    /// </summary>
    private void RunScript(
        Mesh mesh, 
        List<Vector3> dir, 
        List<float> dirF, 
        List<float> v, 
        List<float> dB,
        List<float> feed, 
        List<float> kill, 
        List<int> seedA, 
        List<int> seedB, 
        int iter)
    {
        //Written by Vicente Soler and changed by Laurent Delrieu
        //http://www.grasshopper3d.com/forum/topics/reaction-diffusion-on-triangular-mesh
        //29th of August 2015
        //Adapted by Bathsheba Grossman February 2019 to take more general inputs and use Alea to compute on the GPU.

        if (mesh == null)
        {
            Debug.Log("No mesh");
            return;
        }

        //Validate inputs: supply reasonable defaults for nulls and check # of values.  No range checks.
        var size = mesh.vertexCount;

        if (dir.Count == 0 || dirF.Count == 0)
        {
            //direction vector and weight
            dirF = new List<float> {1.0f};
            dir = new List<Vector3> {new Vector3(1, 0, 0)};
        }
        else
        {
            if (!CheckLen<Vector3>(dir, "dir", size)) return;
        }

        if (v.Count == 0 ) v = new List<float> {1.0f}; //slows diffusion => shrinks scale 0-1
        if (dB.Count == 0 ) dB = new List<float> {0.5f}; //diffusion of B relative to A, always < 1

        if (feed.Count == 0 ) feed = new List<float> {0.055f};
        if (kill.Count == 0 ) kill = new List<float> {0.062f};

        if (seedA.Count == 0 ) seedA = new List<int> {1}; //default A seeds all 1

        if (seedB.Count == 0 )
        {
            //default B seeds 1/20 1 rest 0
            for (var i = 0; i < size; i++)
                seedB.Add((Random.Range(0,1) < 0.05) ? 1 : 0);
        }

        if (!CheckLen<float>(dirF, "dirF", size)) return;
        if (!CheckLen<float>(v, "v", size)) return;
        if (!CheckLen<float>(dB, "dB", size)) return;
        if (!CheckLen<float>(feed, "feed", size)) return;
        if (!CheckLen<float>(kill, "kill", size)) return;
        if (!CheckLen<float>(seedA, "seedA", size)) return;
        if (!CheckLen<float>(seedB, "seedB", size)) return;
        //Done validating

        var t = DateTime.Now;
        var reaction = new ReactionDiffusion(mesh, dir, dirF, v, dB, feed, kill, seedA, seedB);

        var s = DateTime.Now.Subtract(t);
        Debug.Log("initialize: " + s.ToString());

        t = DateTime.Now;
        reaction.Run(iter);

        s = DateTime.Now.Subtract(t);
        Debug.Log("run: " + s.ToString());

        var A = reaction.ListA;
        var B = reaction.ListB;
    }

    // <Custom additional code> 

    private class ReactionDiffusion
    {
        protected readonly List<Particle> particles = new List<Particle>();

        public IEnumerable<float> ListA
        {
            get { return particles.Select(p => p.A).ToList(); }
        }

        public IEnumerable<float> ListB
        {
            get { return particles.Select(p => p.B).ToList(); }
        }

        public ReactionDiffusion(
            Mesh mesh, 
            IReadOnlyList<Vector3> dir, 
            IReadOnlyList<float> dirF, 
            IReadOnlyList<float> v,
            IReadOnlyList<float> dB, 
            IReadOnlyList<float> feed, 
            IReadOnlyList<float> kill, 
            IReadOnlyList<int> seedA, 
            IReadOnlyList<int> seedB)
        {
            var size = mesh.vertexCount;

            for (var i = 0; i < size; i++) //make particles
            {
                float a = (seedA.Count == 1 ? seedA[0] : seedA[i]); //a and b starts
                float b = seedB[i];
                var f = feed.Count == 1 ? feed[0] : feed[i]; //f and k
                var k = kill.Count == 1 ? kill[0] : kill[i];
                var d = dir.Count == 1 ? dir[0] : dir[i]; //direction & strength
                var dFac = dirF.Count == 1 ? dirF[0] : dirF[i];
                var vs = v.Count == 1 ? v[0] : v[i]; //speed of reaction relative to time
                var diffB = dB.Count == 1 ? dB[0] : dB[i]; //diffusion speed of b

                particles.Add(new Particle(a, b, f, k, d, dFac, vs, diffB, mesh.vertices[i], this));
            }

            //find particle neighbors and weights
            System.Threading.Tasks.Parallel.For(0, size, i => particles[i].SetNeighbours(i, mesh));
        }
        
        

        private void FindNeighbours()
        {
            var size = particles.Count;
            
            //make neighbours and weights into non-ragged 2d arrays
            var neighbourLengths = particles.Select(particle => particle.neighbourIndices.Count).ToArray();
            var maxNeighbours = neighbourLengths.Max();

            var neighbours = new int[size, maxNeighbours];
            var weights = new float[size, maxNeighbours];

            for (var i = 0; i < size; i++)
            {
                var p = particles[i];
                for (var j = 0; j < neighbourLengths[i]; j++)
                {
                    //oh c#, no faster way?
                    neighbours[i, j] = p.neighbourIndices[j];
                    weights[i, j] = p.weights[j];
                }
            }
            /*
            var gnblen = gpu.Allocate(neighbourLengths);
            int[,] gnbrs = gpu.Allocate(neighbours); //because these arrays aren't ragged they can be copied en bloc
            float[,] gweights = gpu.Allocate(weights);
            */
        }

        public void Run(int iterations)
        {
            /*//how it used to be
            while(iterations-- > 0)
            {
              System.Threading.Tasks.Parallel.ForEach(particles, particle => particle.Laplacian());
              System.Threading.Tasks.Parallel.ForEach(particles, particle => particle.ReactionDiffusion());
            }*/

            var size = particles.Count;

           
/*
            var gpu = Alea.Gpu.Default; //put particle values into arrays and copy them to the GPU

            var A = gpu.Allocate<float>(particles.Select(particle => particle.A).ToArray());
            var B = gpu.Allocate<float>(particles.Select(particle => particle.B).ToArray());
            var f = gpu.Allocate<float>(particles.Select(particle => particle.f).ToArray());
            var k = gpu.Allocate<float>(particles.Select(particle => particle.k).ToArray());
            var v = gpu.Allocate<float>(particles.Select(particle => particle.v).ToArray());
            var dB = gpu.Allocate<float>(particles.Select(particle => particle.dB).ToArray());

            

            var dxA = gpu.Allocate<float>(new float[size]); //these start at 0
            var dxB = gpu.Allocate<float>(new float[size]);
*/
            while (iterations-- > 0) //do it
            {
                //run laplacians

                //reaction diffusion
            }
            
            

            var resA = new float[size];
            var resB = new float[size];
            /*
            Gpu.CopyToHost(A).CopyTo(resA, 0); //pull results out of GPU
            Gpu.CopyToHost(B).CopyTo(resB, 0);

            for (int i = 0; i < size; i++) //put them back into particles. can't linq do this?
            {
                particles[i].A = resA[i];
                particles[i].B = resB[i];
            }

            Gpu.Free(A); //because we're nice people
            Gpu.Free(B);
            Gpu.Free(dxA);
            Gpu.Free(dxB);
            Gpu.Free(f);
            Gpu.Free(k);
            Gpu.Free(v);
            Gpu.Free(dB);
            Gpu.Free(gnblen);
            Gpu.Free(gnbrs);
            Gpu.Free(gweights);
            */
        }

        protected class Particle
        {
            public float A { get; set; }
            public float B { get; set; }

            private readonly Vector3 _point;
            public readonly float f;
            public readonly float k;
            public readonly float v;
            public readonly float dB;
            private readonly float _dirF;
            private readonly Vector3 _dir;

            public readonly List<int> neighbourIndices = new List<int>(); //neighbour indices
            public readonly List<float> weights = new List<float>();
            public float weightTotal;
            ReactionDiffusion reaction;

            public Particle(float a, float b, float f, float k, Vector3 dir, float dirF, float v, float dB,
                Vector3 point, ReactionDiffusion reacc)
            {
                this.A = a;
                this.B = b;
                this.f = f;
                this.k = k;
                this._dir = dir;
                this._dirF = dirF;
                this.v = v;
                this.dB = dB;
                this._point = point;
                this.reaction = reacc;
            }

            public void SetNeighbours(int i, Mesh mesh)
            {
                /*
                foreach (int j in mesh.vertices.GetConnectedVertices(i).Where(x => x != i))
                {
                    neighbourIndices.Add(j);

                    float angle = Vector3d.VectorAngle(reaction.particles[j]._point - _point, _dir);
                    //if dirF == 1 weight is 1. > 1, higher weight if parallel with curve.  <1, higher weight if perpendicular to curve.
                    var s = Mathf.Sin(angle);
                    var c = Mathf.Cos(angle);
                    var weight = Mathf.Sqrt(s * s + _dirF * _dirF * c * c);
                    weights.Add(weight);
                    weightTotal += weight;
                }

                for (var j = 0; j < weights.Count; j++)
                    weights[j] /= weightTotal;
            */
            }
        } //end Particle
    } //end ReactionDiffusion

    private static bool CheckLen<T>(ICollection stuff, string name, int size)
    {
        if (stuff.Count == 1 || stuff.Count == size) return true;
        
        Debug.Log(name + " - wrong number of values, should be 1 or "+ size);
        return false;

    }
}