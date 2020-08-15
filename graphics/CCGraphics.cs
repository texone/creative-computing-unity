using System;
using cc.creativecomputing.math.util;

namespace UnityEngine
{
	public enum CCShapeMode{
		CORNER,CORNERS,RADIUS,CENTER
	}
	
	public enum CCTextAlign{
		RIGHT, LEFT,CENTER, JUSTIFY
	}

public enum CCDrawMode : int{
	
	/**
	 * Treats each pair of vertices as an independent line segment. 
	 * Vertices 2n-1 and 2n define line n. N/2 lines are drawn.
	 */
	LINES = GL.LINES,
	/**
	 * Draws a connected group of line segments from the first vertex to the last. 
	 * Vertices n and n+1 define line n. N-1 lines drawn.
	 */
	LINE_STRIP = GL.LINE_STRIP,

	/**
	 * Treates each triplet of vertices as an independent triangle. 
	 * Vertices 3n-2, 3n-1, and 3n define triangle n. N/3 triangles are drawn.
	 */
	TRIANGLES = GL.TRIANGLES,
	/**
	 * Draws a connected group of triangles. One triangle is defined for each 
	 * vertex presented after the first two vertices. 
	 * For odd n, vertices n, n+1, and n+2 define triangle n. 
	 * For even n, vertices n+1, n, and n+2 define triangle n. N-2 triangles are drawn.
	 */
	TRIANGLE_STRIP = GL.TRIANGLE_STRIP,
	/**
	 * Treats each group of four vertices as an independent quadrilateral. 
	 * Vertices 4n-3, 4n-2, 4n-1, and 4n define quadrilateral n. N/4 quadrilaterals are drawn.
	 */
	QUADS = GL.QUADS,
	
	
}
	
    public class CCGraphics
    {
	    
	    private CCShapeMode _myImageMode = CCShapeMode.CORNER;
	    private CCShapeMode _myRectMode = CCShapeMode.CORNER;
	    private CCShapeMode _myEllipseMode = CCShapeMode.RADIUS;
        //////////////////////////////////////////////////////////////
	//
	// ELLIPSE AND ARC
	//
	/////////////////////////////////////////////////////////////

	/**
	 * The origin of the ellipse is modified by the ellipseMode() function.
	 * The possible Modes are:
	 * <ul>
	 * <li>CENTER: the default configuration, specifies the location of the ellipse as the center of the shape.</li>
	 * <li>RADIUS: the same like CENTER but width and height define radius of the ellipse rather than the diameter</li>
	 * <li>CORNER: draws the shape from the upper-left corner of its bounding box.</li>
	 * <li>CORNERS: uses the four parameters to ellipse() to set two opposing corners of the ellipse's bounding box</li>
	 * </ul>
	 * @param theMode, SHAPEMODE: Either CENTER, RADIUS, CORNER, or CORNERS.
	 * @related ellipse ( )
	 */
	public void EllipseMode(CCShapeMode theMode) {
		_myEllipseMode = theMode;
	}
	
	public CCShapeMode EllipseMode(){
		return _myEllipseMode;
	}
	
	// precalculate sin/cos lookup tables
	// circle resolution is determined from the actual used radii
	// passed to ellipse() method. this will automatically take any
	// scale transformations into account too
	private static readonly float[] SinLut;
	private static readonly float[] CosLut;
	private const float sinCosPrecision = 0.5f;
	private const int sinCosLength = (int) (360f / sinCosPrecision);
	private static Material colorMaterial;
	
	private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
	private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
	private static readonly int Cull = Shader.PropertyToID("_Cull");
	private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
	
	static CCGraphics(){
		SinLut = new float[sinCosLength];
		CosLut = new float[sinCosLength];
		
		for (var i = 0; i < sinCosLength; i++) {
			SinLut[i] = CCMath.Sin(i * CCMath.DEG_TO_RAD * sinCosPrecision);
			CosLut[i] = CCMath.Cos(i * CCMath.DEG_TO_RAD * sinCosPrecision);
		}
		
		// Unity has a built-in shader that is useful for drawing
		// simple colored things.
		var shader = Shader.Find("Hidden/Internal-Colored");
		colorMaterial = new Material(shader) {hideFlags = HideFlags.HideAndDontSave};
		// Turn on alpha blending
		colorMaterial.SetInt(SrcBlend, (int)Rendering.BlendMode.SrcAlpha);
		colorMaterial.SetInt(DstBlend, (int)Rendering.BlendMode.OneMinusSrcAlpha);
		// Turn backface culling off
		colorMaterial.SetInt(Cull, (int)Rendering.CullMode.Off);
		// Turn off depth writes
		colorMaterial.SetInt(ZWrite, 0);
	}

	public void BeginDraw()
	{
		// Apply the line material
		colorMaterial.SetPass(0);
		
		GL.PushMatrix();
		GL.LoadIdentity();
		var proj = Matrix4x4.Ortho(0, Screen.width, 0, Screen.height, -1, 100);
		GL.LoadProjectionMatrix(proj);
	}

	public void EndDraw()
	{
		GL.PopMatrix();
	}
	
	/**
	 * Use this method to set the drawing color, everything you draw
	 * after a call of color, will have the defined color, there are three
	 * ways to define a color, first is to use double values between 0 and 1,
	 * the second is to use integer values between 0 and 255 and the third way
	 * is to use the CCColor class.
	 * @param theRed
	 * @param theGreen
	 * @param theBlue
	 * @param theAlpha
	 */
	public void Color(float theRed, float theGreen, float theBlue, float theAlpha = 1){
		Color(new Color(theRed, theGreen, theBlue, theAlpha));
	}

	public void Color(float theGray, float theAlpha = 1) {
		Color(theGray,theGray,theGray,theAlpha);
	}

	private Color _myColor = new Color(1f,1f,1f,1f);
	public void Color(Color color)
	{
		_myColor = color;
		if (_myIsInShape)
		{
			GL.Color(_myColor);
		}
	}
	
	public void Color(Color color, float theAlpha){
		Color(new Color(color.r, color.g, color.b, theAlpha));
	}
	
	public void Color(int theRGB) {
		if (((theRGB & 0xff000000) == 0) && (theRGB <= 255)) {
			Color(theRGB, theRGB, theRGB);
		} else {
			Color(
				(theRGB >> 16) & 0xff,
				(theRGB >> 8)  & 0xff,
				(theRGB)	   & 0xff,	
				(theRGB >> 24) & 0xff
			);
		}
	}
	
	public void Color(int theRed, int theGreen, int theBlue, int theAlpha = 255){
		Color(theRed / 255f, theGreen / 255f, theBlue / 255f, theAlpha / 255f);
	}
	
	public void Color(int theGray, int theAlpha){
		Color(theGray,theGray,theGray,theAlpha);
	}

	private bool _myIsInShape = false;
	public void BeginShape(CCDrawMode theMode)
	{
		GL.Begin((int)theMode);
		_myIsInShape = true;
	}

	public void EndShape()
	{
		GL.End();
		_myIsInShape = false;
	}

	public void Vertex(float theX, float theY, float theZ = 0)
	{
		GL.Vertex3(theX, theY, theZ);
	}

	public void Vertex(Vector3 theVertex)
	{
		GL.Vertex(theVertex);
	}
	
	/////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////
	//
	// 2D PRIMITIVES
	//
	/////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////

	/**
	 * Draws a line (a direct path between two points) to the screen. The version of line() 
	 * with four parameters draws the line in 2D. To color a line, use the color() function. 
	 * 2D lines are drawn with a width of one pixel by default, but this can be changed with 
	 * the lineWidth() function. The version with six parameters allows the line to be placed 
	 * anywhere within XYZ space.
	 * @param x1 x coordinate of the lines starting point
	 * @param y1 y coordinate of the lines starting point
	 * @param x2 x coordinate of the lines end point
	 * @param y2 y coordinate of the lines end point
	 */
	public void Line(float x1, float y1, float x2, float y2) {
		BeginShape(CCDrawMode.LINES);
		Color(_myColor);
		Vertex(x1, y1);
		Vertex(x2, y2);
		EndShape();
	}
	
	/**
	 * @param z1 z coordinate of the lines starting point
	 * @param z2 z coordinate of the lines end point
	 */
	public void Line(float x1, float y1, float z1, float x2, float y2, float z2) {
		BeginShape(CCDrawMode.LINES);
		Color(_myColor);
		Vertex(x1, y1, z1);
		Vertex(x2, y2, z2);
		EndShape();
	}
	
	/**
	 * @param v1 vector with the x,y coordinates of the lines start point
	 * @param v2 vector with the x,y coordinates of the lines end point
	 */
	public void Line(Vector2 v1, Vector2 v2) {
		BeginShape(CCDrawMode.LINES);
		Color(_myColor);
		Vertex(v1);
		Vertex(v2);
		EndShape();
	}
	
	/**
	 * @param v1 CCVector3: vector with the x,y,z coordinates of the lines start point
	 * @param v2 CCVector3: vector with the x,y,z coordinates of the lines end point
	 */
	public void Line(Vector3 v1, Vector3 v2) {
		BeginShape(CCDrawMode.LINES);
		Color(_myColor);
		Vertex(v1);
		Vertex(v2);
		EndShape();
	}

	public void Ellipse(float theX, float theY, float theZ, float theWidth, float theHeight, bool theDrawOutline = false) {
	    var myX = theX;
	    var myY = theY;
	    var myWidth = theWidth;
	    var myHeight = theHeight;
	    
	    switch(_myEllipseMode){
	    	case CCShapeMode.CORNERS:
	    		myWidth = theWidth - theX;
	    		myHeight = theHeight - theY;
	    		break;
	    	case CCShapeMode.RADIUS:
	    		myX = theX - theWidth;
	    		myY = theY - theHeight;
	    		myWidth = theWidth * 2;
	    		myHeight = theHeight * 2;
	    		break;
	        case CCShapeMode.CORNER:
	        case CCShapeMode.CENTER:
		        break;
	        default:
	    		myX = theX - theWidth/2f;
	    		myY = theY - theHeight/2f;
	            break;
	    }
	    // undo negative width
		if (myWidth < 0) { 
			myX += myWidth;
			myWidth = -myWidth;
		}

		// undo negative height
		if (myHeight < 0) { 
			myY += myHeight;
			myHeight = -myHeight;
		}

		myWidth /= 2;
		myHeight /= 2;

	    myX += myWidth;
	    myY += myHeight;

	    var accuracy = (int)(4+CCMath.Sqrt(myWidth+myHeight)*3);
	    var inc = (float)sinCosLength / accuracy;
	    
	    if(theDrawOutline){
		    GL.Begin(GL.LINE_STRIP);
		    float val = 0;
		    GL.Color(_myColor);
		    for (var i = 0; i < accuracy; i++) {
			    GL.Vertex3(
				    myX + CosLut[(int) val] * myWidth, 
				    myY + SinLut[(int) val] * myHeight,
				    theZ
			    );
			    val += inc;
		    }
		    // back to the beginning
		    GL.Vertex3(myX + CosLut[0] * myWidth, myY + SinLut[0] * myHeight,theZ);
		    GL.End();
	    } else {
		    GL.Begin(GL.TRIANGLE_STRIP);
		    GL.Color(_myColor);
		    float val = 0;
		    for (var i = 0; i < accuracy; i++) {
			    GL.Vertex3(myX, myY,theZ);
			    GL.Vertex3(
				    myX + CosLut[(int) val] * myWidth, 
				    myY + SinLut[(int) val] * myHeight,
				    theZ
			    );
			    val += inc;
		    }
		    // back to the beginning
		    GL.Vertex3(myX + CosLut[0] * myWidth, myY + SinLut[0] * myHeight,theZ);
		    GL.End();
	    }
	    
	}
	
	/**
	 * Draws an ellipse (oval) in the display window. An ellipse with an equal
	 * width and height is a circle. The first two parameters set the location,
	 * the third sets the width, and the fourth sets the height. The origin may
	 * be changed with the ellipseMode() function.
	 * 
	 * @param theX
	 * @param theY
	 * @param theWidth
	 * @param theHeight
	 */
	public void Ellipse(float theX, float theY, float theZ, float theWidth, float theHeight) {
	    Ellipse(theX, theY, theZ, theWidth, theHeight, false);
	}
	
	public void Ellipse(float theX, float theY, float theWidth, float theHeight){
		Ellipse(theX, theY, 0, theWidth, theHeight);
	}
	
	public void Ellipse(float theX, float theY, float theDiameter){
		Ellipse(theX,theY,0,theDiameter,theDiameter);
	}
	
	public void Ellipse(Vector2 thePosition, float theDiameter){
		Ellipse(thePosition.x, thePosition.y, theDiameter, theDiameter);
	}
	
	public void Ellipse(Vector2 thePosition, float theWidth, float theHeight){
		Ellipse(thePosition.x, thePosition.y, theWidth, theHeight);
	}
	
	public void Ellipse(Vector2 thePosition, float theWidth, float theHeight, bool theDrawOutline = false){
		Ellipse(thePosition.x, thePosition.y, 0, theWidth, theHeight, theDrawOutline);
	}
	
	public void Ellipse(Vector3 thePosition, float theDiameter){
		Ellipse(thePosition.x, thePosition.y, thePosition.z, theDiameter, theDiameter);
	}
	
	public void Ellipse(Vector3 thePosition, float r0, float r1){
		Ellipse(thePosition.x, thePosition.y, thePosition.z, r0, r1);
	}

	/**
	 * Draws an arc in the display window. Arcs are drawn along the outer edge of 
	 * an ellipse defined by the x, y, width and height parameters. 
	 * The origin or the arc's ellipse may be changed with the ellipseMode() function. 
	 * The start and stop parameters specify the angles at which to draw the arc.
	 * @param theX
	 * @param theY
	 * @param theWidth
	 * @param theHeight
	 * @param theStart
	 * @param theStop
	 * @related ellipse()
	 * @related ellipseMode()
	 */
	public void Arc(
		float theX, float theY, 
		float theWidth, float theHeight,
		float theStart, float theStop
	) {
		var x = theX;
		var y = theY;
		var w = theWidth;
		var h = theHeight;

		switch(_myEllipseMode){
    	case CCShapeMode.CORNERS:
    		w = theWidth - theX;
  	      	h = theHeight - theY;
    		break;
    	case CCShapeMode.RADIUS:
    		x = theX - theWidth;
    		y = theY - theHeight;
    		w = theWidth * 2;
    		h = theHeight * 2;
    		break;
    	case CCShapeMode.CENTER:
    		x = theX - theWidth/2f;
    		y = theY - theHeight/2f;
    		break;
    	default:
	        break;
		}

		// if (angleMode == DEGREES) {
		// start = start * DEG_TO_RAD;
		// stop = stop * DEG_TO_RAD;
		// }
		// before running a while loop like this,
		// make sure it will exit at some point.
		if (float.IsInfinity(theStart) || float.IsInfinity(theStop)){
			return;
		}
		
		while (theStop < theStart){
			theStop += CCMath.TWO_PI;
		}
		
		// undo negative width
		if (w < 0) { 
			x += w;
			w = -w;
		}

		// undo negative height
		if (h < 0) { 
			y += h;
			h = -h;
		}

		var hr = w / 2f;
		var vr = h / 2f;

		var centerX = x + hr;
		var centerY = y + vr;

		var startLut = (int) (0.5f + (theStart / CCMath.TWO_PI) * sinCosLength);
		var stopLut = (int) (0.5f + (theStop / CCMath.TWO_PI) * sinCosLength);

		GL.Begin(GL.TRIANGLE_STRIP);
		const int increment = 1; // what's a good algorithm? stopLUT - startLUT;
		
		for (var i = startLut; i < stopLut; i += increment) {
			GL.Color(_myColor);
			GL.Vertex3(centerX, centerY,0);
			var ii = i % sinCosLength;
			GL.Vertex3(centerX + CosLut[ii] * hr, centerY + SinLut[ii] * vr,0);
		}
		// draw last point explicitly for accuracy
		GL.Vertex3(
			centerX + CosLut[stopLut % sinCosLength] * hr, 
			centerY+ SinLut[stopLut % sinCosLength] * vr,
			0
		);
		GL.End();
	}
	
	//////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////
	//
	// TEXTHANDLING
	//
	//////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////
	
	private CCTextAlign _myAlign = CCTextAlign.LEFT;
	
	public void TextAlign(CCTextAlign theTextAlign){
		_myAlign = theTextAlign;
	}
	
	public CCTextAlign TextAlign(){
		return _myAlign;
	}

	/**
	 * Set the text leading to a specific value. If using a custom value for
	 * the text leading, you'll have to call textLeading() again after any
	 * calls to textSize().
	 */
	public float TextLeading { get; set; } = 20 * 1.275f;

	private float _myTextSize = 20;

	/**
	 * Same as parent, but override for native version of the font. <p/> Also
	 * gets called by textFont, so the metrics will get recorded properly.
	 */
	public float TextSize
	{
		get => _myTextSize;
		set
		{
			_myTextSize = value;
			TextLeading = _myTextSize * 1.275f;
		}
	}

	private CCMonoLineFont _myFont = new CCMonoLineFont();

	public void Text(string theString, float theX, float theY, float theZ = 0){
		if(theString == null)return;
		
		PushMatrix();
		Translate(theX, theY, theZ);
		Scale(TextSize, TextSize);
		_myFont.Draw(this, _myColor, theString, TextLeading / _myTextSize, 1f);
		PopMatrix();
	}
	
	public void Text(char theChar, float theX, float theY, float theZ = 0){
		Text(theChar.ToString(), theX, theY, theZ);
	}
	
	public void Text(string theText, Vector3 theVector) {
		Text(theText, theVector.x, theVector.y, theVector.z);
	}
	
	public void Text(string theString, Vector2 theVector){
		Text(theString, theVector.x, theVector.y);
	}

	public void Text(int theNumber, float theX, float theY, float theZ = 0){
		Text(theNumber + "", theX, theY, theZ);
	}

	public void Text(float theNumber, float theX, float theY, float theZ = 0){
		Text(theNumber+"", theX, theY, theZ);
	}

	public void Text(int theNumber, Vector2 theVector){
		Text(theNumber + "", theVector.x, theVector.y);
	}

	public void Text(int theNumber, Vector3 theVector){
		Text(theNumber + "", theVector.x, theVector.y, theVector.z);
	}
	
	/////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////
	//
	//  MATRIX OPERATIONS
	//
	/////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////
	
	// public enum CCMatrixMode{
	// 	/**
	// 	 * Applies subsequent matrix operations to the modelview matrix stack.
	// 	 */
	// 	MODELVIEW(GLMatrixFunc.GL_MODELVIEW,GLMatrixFunc.GL_MODELVIEW_MATRIX),
	// 	/**
	// 	 * Applies subsequent matrix operations to the projection matrix stack.
	// 	 */
	// 	PROJECTION(GLMatrixFunc.GL_PROJECTION,GLMatrixFunc.GL_PROJECTION_MATRIX),
	// 	/**
	// 	 * Applies subsequent matrix operations to the texture matrix stack.
	// 	 */
	// 	TEXTURE(GL.GL_TEXTURE,GLMatrixFunc.GL_TEXTURE_MATRIX);
	// 	
	// 	int glID;
	// 	int glMatrixID;
	// 	
	// 	CCMatrixMode(final int theGlID, final int theGlMatrixID){
	// 		glID = theGlID;
	// 		glMatrixID = theGlMatrixID;
	// 	}
	// }
	
	
	/**
	 * Replaces the current matrix with the identity matrix. It is semantically 
	 * equivalent to calling glLoadMatrix with the identity matrix.
	 * Use the loadIdentity() command to clear the currently modifiable matrix 
	 * for future transformation commands, since these commands modify the current 
	 * matrix. Typically, you always call this command before specifying projection 
	 * or viewing transformations, but you might also call it before specifying 
	 * a modeling transformation.
	 */
	public void LoadIdentity(){
		modelview = Matrix4x4.identity;
		GL.LoadIdentity();
	}
	
	
	/**
	 * Replaces the current matrix with the one specified in m. The current matrix 
	 * is the projection matrix, modelview matrix, or texture matrix, determined 
	 * by the current matrix mode.
	 * @param theMatrix Matrix4f, matrix the current matrix is set to
	 * @related matrixMode ( )
	 */
	public void LoadMatrix(Matrix4x4 theMatrix){
		GL.LoadIdentity();
		GL.MultMatrix(theMatrix);
	}
	
	/**
	 * Applies the matrix specified by the sixteen values pointed to by m by the 
	 * current matrix and stores the result as the current matrix.
	 * @param theMatrix
	 */
	public void ApplyMatrix(Matrix4x4 theMatrix)
	{
		modelview *= theMatrix;
		GL.MultMatrix(modelview);
	}
	
	public void ApplyTransform(Transform theTransform)
	{
		ApplyMatrix(theTransform.localToWorldMatrix);
	}
	

	/*
	public void applyMatrix(float[] theMatrix) {
		ApplyMatrix();
	}
	
	public void applyMatrix(
		float n00, float n01, float n02, float n03,
		float n10, float n11, float n12, float n13,
		float n20, float n21, float n22, float n23,
		float n30, float n31, float n32, float n33
	){
		final DoubleBuffer myMatrixBuffer = DoubleBuffer.allocate(16);
		
		myMatrixBuffer.put(n00); myMatrixBuffer.put(n10); myMatrixBuffer.put(n20); myMatrixBuffer.put(n30);
		myMatrixBuffer.put(n01); myMatrixBuffer.put(n11); myMatrixBuffer.put(n21); myMatrixBuffer.put(n31);
		myMatrixBuffer.put(n02); myMatrixBuffer.put(n12); myMatrixBuffer.put(n22); myMatrixBuffer.put(n32);
		myMatrixBuffer.put(n03); myMatrixBuffer.put(n13); myMatrixBuffer.put(n23); myMatrixBuffer.put(n33);

		myMatrixBuffer.rewind();
		gl.glMultMatrixd(myMatrixBuffer);
	}*/
	

	  /**
		 * Apply a 3x2 affine transformation matrix.
		 */
	  /*
	public void applyMatrix(
		float n00, float n01, float n02, 
		float n10, float n11, float n12
	){
		final DoubleBuffer myMatrixBuffer = DoubleBuffer.allocate(16);
		
		myMatrixBuffer.put(n00); myMatrixBuffer.put(n10); myMatrixBuffer.put(0); myMatrixBuffer.put(0);
		myMatrixBuffer.put(n01); myMatrixBuffer.put(n11); myMatrixBuffer.put(0); myMatrixBuffer.put(0);
		myMatrixBuffer.put(0); myMatrixBuffer.put(0); myMatrixBuffer.put(1); myMatrixBuffer.put(0);
		myMatrixBuffer.put(n02); myMatrixBuffer.put(n12); myMatrixBuffer.put(0); myMatrixBuffer.put(1);

		myMatrixBuffer.rewind();
		gl.glMultMatrixd(myMatrixBuffer);
	}*/

	public void ResetMatrix(){
		LoadIdentity();
	}
	
	/**
	 * Moves the coordinate system origin to the specified point.
	 * The point can be passed as vector or separate values and can be 2d or 3d.
	 * If the matrix mode is either MODELVIEW or PROJECTION, all objects drawn 
	 * after translate is called are translated. Use pushMatrix and popMatrix to 
	 * save and restore the untranslated coordinate system.
	 * @shortdesc Moves the coordinate system origin to the point defined point.
	 * @param theX double, x coord of the translation vector
	 * @param theY double, y coord of the translation vector
	 * @param theZ double, z coord of the translation vector
	 */
	public void Translate(float theX, float theY, float theZ = 0){
		Translate(new Vector3(theX, theY, theZ));
	}
	
	public void Translate(Vector3 theVector){
		ApplyMatrix(Matrix4x4.Translate(theVector));
	}
	
	/**
	 * @param theVector Vector2f, the translation vector
	 */
	public void Translate(Vector2 theVector){
		ApplyMatrix(Matrix4x4.Translate(theVector));
	}
	
	public void Rotate(Quaternion theQuaternion){
		ApplyMatrix(Matrix4x4.Rotate(theQuaternion));
	}
	
	/**
	 * @param theVector Vector3f, vector 
	 */
	public void Rotate(float theAngle,  Vector3 theVector) {
		ApplyMatrix(Matrix4x4.Rotate(Quaternion.AngleAxis(theAngle, theVector)));
	}
	
	
	/**
	 * Multiplies the current matrix by a matrix that rotates an object 
	 * (or the local coordinate system) in a counterclockwise direction about 
	 * the ray from the origin through the point (x, y, z). The angle parameter 
	 * specifies the angle of rotation in degrees.<br>
	 * If the matrix mode is either MODELVIEW or PROJECTION, all objects drawn 
	 * after rotate is called are rotated. Use pushMatrix and popMatrix to save 
	 * and restore the unrotated coordinate system.
	 * @param theAngle double, the angle of rotation, in degrees.
	 * @param theX double, x coord of the vector
	 * @param theY double, y coord of the vector
	 * @param theZ double, z coord of the vector
	 */
	public void Rotate(float theAngle, float theX, float theY, float theZ) {
		Rotate(theAngle, new Vector3(theX, theY, theZ));
	}

	/**
	 * Rotates an object around the X axis the amount specified by the angle parameter.
	 * Objects are always rotated around their relative position to the origin and positive 
	 * numbers rotate objects in a counterclockwise direction. Transformations apply to 
	 * everything that happens after and subsequent calls to the function accumulates the effect.
	 * @param theAngle double, the angle of rotation, in degrees.
	 */
	public void RotateX(float theAngle){
		Rotate(theAngle,1.0f,0.0f,0.0f);
	}
	
	/**
	 * Rotates an object around the Y axis the amount specified by the angle parameter.
	 * Objects are always rotated around their relative position to the origin and positive 
	 * numbers rotate objects in a counterclockwise direction. Transformations apply to 
	 * everything that happens after and subsequent calls to the function accumulates the effect.
	 * @param theAngle double, the angle of rotation, in degrees.
	 */
	public void RotateY(float theAngle){
		Rotate(theAngle,0.0f,1.0f,0.0f);
	}
	
	/**
	 * Rotates an object around the Z axis the amount specified by the angle parameter.
	 * Objects are always rotated around their relative position to the origin and positive 
	 * numbers rotate objects in a counterclockwise direction. Transformations apply to 
	 * everything that happens after and subsequent calls to the function accumulates the effect.
	 * @param theAngle double, the angle of rotation, in degrees.
	 */
	public void RotateZ(float theAngle){
		Rotate(theAngle,0.0f,0.0f,1.0f);
	}
	
	/**
	 * Rotates an object around the Z axis the amount specified by the angle parameter.
	 * Objects are always rotated around their relative position to the origin and positive 
	 * numbers rotate objects in a counterclockwise direction. Transformations apply to 
	 * everything that happens after and subsequent calls to the function accumulates the effect.
	 * @param theAngle double, the angle of rotation, in degrees.
	 */
	public void Rotate(float theAngle){
		Rotate(theAngle,0.0f,0.0f,1.0f);
	}
	
	/**
	 * Produces a general scaling along the x, y, and z axes. The three arguments indicate 
	 * the desired scale factors along each of the three axes. If the matrix mode is either 
	 * MODELVIEW or PROJECTION, all objects drawn after scale is called are scaled. Use 
	 * pushMatrix and popMatrix to save and restore the unscaled coordinate system.<br>
	 * Scale() is the only one of the three modeling transformations that changes the apparent 
	 * size of an object: Scaling with values greater than 1.0 stretches an object, and using 
	 * values less than 1.0 shrinks it. Scaling with a -1.0 value reflects an object across an 
	 * axis. The identity values for scaling are (1.0, 1.0, 1.0). In general, you should limit 
	 * your use of scale() to those cases where it is necessary. Using scale() decreases the 
	 * performance of lighting calculations, because the normal vectors have to be renormalized 
	 * after transformation.<br>
	 * A scale value of zero collapses all object coordinates along that axis to zero. It's 
	 * usually not a good idea to do this, because such an operation cannot be undone. 
	 * Mathematically speaking, the matrix cannot be inverted, and inverse matrices are required 
	 * for certain lighting operations. Sometimes collapsing coordinates does make sense, however; 
	 * the calculation of shadows on a planar surface is a typical application. In general, if a 
	 * coordinate system is to be collapsed, the projection matrix should be used rather than 
	 * the modelview matrix. 
	 * @param theX double, scale factor along the x axis
	 * @param theY double, scale factor along the y axis
	 * @param theZ double, scale factor along the z axis
	 */
	public void Scale(float theX, float theY, float theZ = 1) {
		ApplyMatrix(Matrix4x4.Scale(new Vector3(theX, theY, theZ)));
	}

	

	public void Scale(float theSize) {
		Scale(theSize, theSize, theSize);
	}
	
	static  protected int MATRIX_STACK_DEPTH = 32;

	static  protected string ERROR_PUSHMATRIX_OVERFLOW =
		"Too many calls to pushMatrix().";
	static  protected string ERROR_PUSHMATRIX_UNDERFLOW =
		"Too many calls to popMatrix(), and not enough to pushMatrix().";
	protected int modelviewStackDepth;
	protected int projectionStackDepth;

	/** Modelview matrix stack **/
	protected Matrix4x4[] modelviewStack = new Matrix4x4[MATRIX_STACK_DEPTH];

	/** Inverse modelview matrix stack **/
	//protected Matrix4x4[] modelviewInvStack = new Matrix4x4[MATRIX_STACK_DEPTH];

	/** Camera matrix stack **/
	protected Matrix4x4[] cameraStack = new Matrix4x4[MATRIX_STACK_DEPTH];

	/** Inverse camera matrix stack **/
	//protected Matrix4x4[] cameraInvStack = new Matrix4x4[MATRIX_STACK_DEPTH];

	/** Projection matrix stack **/
	//protected Matrix4x4[] projectionStack = new Matrix4x4[MATRIX_STACK_DEPTH];
	
	public Matrix4x4 projection;
	public Matrix4x4 camera;
	public Matrix4x4 cameraInv;
	public Matrix4x4 modelview = Matrix4x4.identity;
	//public Matrix4x4 modelviewInv;
	public Matrix4x4 projmodelview;
	
	public void PushMatrix() {
		if (modelviewStackDepth == MATRIX_STACK_DEPTH) {
			throw new Exception(ERROR_PUSHMATRIX_OVERFLOW);
		}
		modelviewStack[modelviewStackDepth] = modelview;
		//modelviewInvStack[modelviewStackDepth] = modelviewInv;
		cameraStack[modelviewStackDepth] = camera;
		//cameraInvStack[modelviewStackDepth] = cameraInv;
		modelviewStackDepth++;
	}


	public void PopMatrix() {
		if (modelviewStackDepth == 0) {
			throw new Exception(ERROR_PUSHMATRIX_UNDERFLOW);
		}
		modelviewStackDepth--;
		modelview = modelviewStack[modelviewStackDepth];
		//modelviewInv = modelviewInvStack[modelviewStackDepth];
		camera = cameraStack[modelviewStackDepth];
		//cameraInv = cameraInvStack[modelviewStackDepth];
		
		GL.MultMatrix(modelview);
		//updateProjmodelview();
	}
	
    }
    
    

}