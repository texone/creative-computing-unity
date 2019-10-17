
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine;
using System.Globalization;
using UnityEngine.Playables;

namespace cc.creativecomputing.effects
{

    [AddComponentMenu("effects/csv recorder")]
    public class CCCSVRecorder : MonoBehaviour
    {
        private CCEffects effects;

        private List<List<string>> rowData = new List<List<string>>();

        public float recordingTime = 120;

        public float _myTime = 0;

        public float maxValue = 0;

        public float startTime = 0;

        public PlayableDirector director;

        public int captureRate = 5;

        private int _myEntries = 0;

        public string filename = "export.csv";

        private bool isDone = false;

        // Start is called before the first frame update
        void Start()
        {
            rowData.Clear();
            
            effects = GetComponent<CCEffects>();

            if (effects == null) return;
            Time.captureFramerate = captureRate;

            if(director != null) director.time = startTime;

            _myEntries = effects.effectNodes.Count;
            _myTime = 0;
            isDone = false;
        }

        private void OnEnable()
        {

            Time.captureFramerate = captureRate;
        }

        private void OnDisable()
        {

            Time.captureFramerate = 0;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (isDone) return;
            if (effects == null) return;
            var myAngles = new List<string>();
            effects.effectNodes.ForEach(element =>
            {
                CCEffectData myData = element.GetComponent<CCEffectData>();
                float myAngle = myData.angle * Mathf.Rad2Deg;
                maxValue = Mathf.Max(Mathf.Abs(myAngle), maxValue);
                myAngles.Add(myAngle.ToString(CultureInfo.CreateSpecificCulture("en-EN")));


            });
            rowData.Add(myAngles);

            _myTime += Time.deltaTime;
            if (_myTime >= recordingTime)
            {
                Save();
                isDone = true;
            }
            Debug.Log(Time.deltaTime);
        }

        void Save()
        {


            string[][] output = new string[rowData.Count][];

            StringBuilder sb = new StringBuilder();
            string delimiter = ",";
            rowData.ForEach(row =>
            {
                string myLine = string.Join(delimiter, row);
                sb.AppendLine(myLine);
                Debug.Log(delimiter);
            });

            string filePath = getPath();

            StreamWriter outStream = System.IO.File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
        }

        // Following method is used to retrive the relative path as device platform
        private string getPath()
        {
#if UNITY_EDITOR
            return Application.dataPath + "/CSV/" + filename;
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath +"/"+"Saved_data.csv";
#endif
        }

    }
}
