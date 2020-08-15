using System.Collections.Generic;

namespace UnityEngine
{
    
    public class CCMonoLineChar
    {

        public (int x , int y)[] points;

        public CCMonoLineChar(params (int,int)[] theRatios)
        {
            points = theRatios;
        }
        public void Draw(CCGraphics g, float theX, float theY)
        {
            var x = -1f;
            var y = -1f;

            foreach(var (px,py) in points)
            {
              
                if (px == -1)
                {
                    x = -1;
                    y = -1;
                    continue;
                }

                var dx = px / 8f;
                var dy = py / 12f;

                if (x >= 0)
                {
                    g.Vertex(theX + x, theY + y,0);
                    g.Vertex(theX + dx,theY + dy,0);
                }

                x = dx;
                y = dy;
            }
        }
    }
    
    public class CCMonoLineFont
    {
	    private static readonly (int, int) FontUp = (-1,-1);
	    private readonly CCMonoLineChar[] _charSet = new CCMonoLineChar[128];

        public CCMonoLineFont()
        {
            _charSet['+'] = new CCMonoLineChar((2,1),(2,5),(1,3),(3,3));
            _charSet['-'] = new CCMonoLineChar((1,3),(3,3));
            
            _charSet['0'] = new CCMonoLineChar( (0,0), (8,0), (8,12), (0,12), (0,0), (8,12) );
			_charSet['1'] = new CCMonoLineChar( (4,0), (4,12), (3,10) );
			_charSet['2'] = new CCMonoLineChar( (0,12), (8,12), (8,7), (0,5), (0,0), (8,0) );
			_charSet['3'] = new CCMonoLineChar( (0,12), (8,12), (8,0), (0,0), FontUp, (0,6), (8,6) );
			_charSet['4'] = new CCMonoLineChar( (0,12), (0,6), (8,6), FontUp, (8,12), (8,0) );
			_charSet['5'] = new CCMonoLineChar( (0,0), (8,0), (8,6), (0,7), (0,12), (8,12) );
			_charSet['6'] = new CCMonoLineChar( (0,12), (0,0), (8,0), (8,5), (0,7) );
			_charSet['7'] = new CCMonoLineChar( (0,12), (8,12), (8,6), (4,0) );
			_charSet['8'] = new CCMonoLineChar( (0,0), (8,0), (8,12), (0,12), (0,0), FontUp, (0,6), (8,6));
			_charSet['9'] = new CCMonoLineChar( (8,0), (8,12), (0,12), (0,7), (8,5) );
			_charSet[' '] = new CCMonoLineChar( );
			_charSet['.'] = new CCMonoLineChar( (3,0), (4,0) );
			_charSet[','] = new CCMonoLineChar( (2,0), (4,2) );
			_charSet['-'] = new CCMonoLineChar( (2,6), (6,6) );
			_charSet['+'] = new CCMonoLineChar( (1,6), (7,6), FontUp, (4,9), (4,3) );
			_charSet['!'] = new CCMonoLineChar( (4,0), (3,2), (5,2), (4,0), FontUp, (4,4), (4,12) );
			_charSet['#'] = new CCMonoLineChar( (0,4), (8,4), (6,2), (6,10), (8,8), (0,8), (2,10), (2,2) );
			_charSet['^'] = new CCMonoLineChar( (2,6), (4,12), (6,6) );
			_charSet['='] = new CCMonoLineChar( (1,4), (7,4), FontUp, (1,8), (7,8) );
			_charSet['*'] = new CCMonoLineChar( (0,0), (4,12), (8,0), (0,8), (8,8), (0,0) );
			_charSet['_'] = new CCMonoLineChar( (0,0), (8,0) );
			_charSet['/'] = new CCMonoLineChar( (0,0), (8,12) );
			_charSet['\\'] = new CCMonoLineChar( (0,12), (8,0) );
			_charSet['@'] = new CCMonoLineChar( (8,4), (4,0), (0,4), (0,8), (4,12), (8,8), (4,4), (3,6) );
			_charSet['$'] = new CCMonoLineChar( (6,2), (2,6), (6,10), FontUp, (4,12), (4,0) );
			_charSet['&'] = new CCMonoLineChar( (8,0), (4,12), (8,8), (0,4), (4,0), (8,4) );
			_charSet['['] = new CCMonoLineChar( (6,0), (2,0), (2,12), (6,12) );
			_charSet[']'] = new CCMonoLineChar( (2,0), (6,0), (6,12), (2,12) );
			_charSet['('] = new CCMonoLineChar( (6,0), (2,4), (2,8), (6,12) );
			_charSet[')'] = new CCMonoLineChar( (2,0), (6,4), (6,8), (2,12) );
			_charSet['{'] = new CCMonoLineChar( (6,0), (4,2), (4,10), (6,12), FontUp, (2,6), (4,6) );
			_charSet['}'] = new CCMonoLineChar( (4,0), (6,2), (6,10), (4,12), FontUp, (6,6), (8,6) );
			_charSet['%'] = new CCMonoLineChar( (0,0), (8,12), FontUp, (2,10), (2,8), FontUp, (6,4), (6,2) );
			_charSet['<'] = new CCMonoLineChar( (6,0), (2,6), (6,12) );
			_charSet['>'] = new CCMonoLineChar( (2,0), (6,6), (2,12) );
			_charSet['|'] = new CCMonoLineChar( (4,0), (4,5), FontUp, (4,6), (4,12) );
			_charSet[':'] = new CCMonoLineChar( (4,9), (4,7), FontUp, (4,5), (4,3) );
			_charSet[';'] = new CCMonoLineChar( (4,9), (4,7), FontUp, (4,5), (1,2) );
			_charSet['"'] = new CCMonoLineChar( (2,10), (2,6), FontUp, (6,10), (6,6) );
			_charSet['\''] = new CCMonoLineChar( (2,6), (6,10) );
			_charSet['`'] = new CCMonoLineChar( (2,10), (6,6) );
			_charSet['~'] = new CCMonoLineChar( (0,4), (2,8), (6,4), (8,8) );
			_charSet['?'] = new CCMonoLineChar( (0,8), (4,12), (8,8), (4,4), FontUp, (4,1), (4,0) );
			_charSet['A'] = new CCMonoLineChar( (0,0), (0,8), (4,12), (8,8), (8,0), FontUp, (0,4), (8,4) );
			_charSet['B'] = new CCMonoLineChar( (0,0), (0,12), (4,12), (8,10), (4,6), (8,2), (4,0), (0,0) );
			_charSet['C'] = new CCMonoLineChar( (8,0), (0,0), (0,12), (8,12) );
			_charSet['D'] = new CCMonoLineChar( (0,0), (0,12), (4,12), (8,8), (8,4), (4,0), (0,0) );
			_charSet['E'] = new CCMonoLineChar( (8,0), (0,0), (0,12), (8,12), FontUp, (0,6), (6,6) );
			_charSet['F'] = new CCMonoLineChar( (0,0), (0,12), (8,12), FontUp, (0,6), (6,6) );
			_charSet['G'] = new CCMonoLineChar( (6,6), (8,4), (8,0), (0,0), (0,12), (8,12) );
			_charSet['H'] = new CCMonoLineChar( (0,0), (0,12), FontUp, (0,6), (8,6), FontUp, (8,12), (8,0) );
			_charSet['I'] = new CCMonoLineChar( (0,0), (8,0), FontUp, (4,0), (4,12), FontUp, (0,12), (8,12) );
			_charSet['J'] = new CCMonoLineChar( (0,4), (4,0), (8,0), (8,12) );
			_charSet['K'] = new CCMonoLineChar( (0,0), (0,12), FontUp, (8,12), (0,6), (6,0) );
			_charSet['L'] = new CCMonoLineChar( (8,0), (0,0), (0,12) );
			_charSet['M'] = new CCMonoLineChar( (0,0), (0,12), (4,8), (8,12), (8,0) );
			_charSet['N'] = new CCMonoLineChar( (0,0), (0,12), (8,0), (8,12) );
			_charSet['O'] = new CCMonoLineChar( (0,0), (0,12), (8,12), (8,0), (0,0) );
			_charSet['P'] = new CCMonoLineChar( (0,0), (0,12), (8,12), (8,6), (0,5) );
			_charSet['Q'] = new CCMonoLineChar( (0,0), (0,12), (8,12), (8,4), (0,0), FontUp, (4,4), (8,0) );
			_charSet['R'] = new CCMonoLineChar( (0,0), (0,12), (8,12), (8,6), (0,5), FontUp, (4,5), (8,0) );
			_charSet['S'] = new CCMonoLineChar( (0,2), (2,0), (8,0), (8,5), (0,7), (0,12), (6,12), (8,10) );
			_charSet['T'] = new CCMonoLineChar( (0,12), (8,12), FontUp, (4,12), (4,0) );
			_charSet['U'] = new CCMonoLineChar( (0,12), (0,2), (4,0), (8,2), (8,12) );
			_charSet['V'] = new CCMonoLineChar( (0,12), (4,0), (8,12) );
			_charSet['W'] = new CCMonoLineChar( (0,12), (2,0), (4,4), (6,0), (8,12) );
			_charSet['X'] = new CCMonoLineChar( (0,0), (8,12), FontUp, (0,12), (8,0) );
			_charSet['Y'] = new CCMonoLineChar( (0,12), (4,6), (8,12), FontUp, (4,6), (4,0) );
			_charSet['Z'] = new CCMonoLineChar( (0,12), (8,12), (0,0), (8,0), FontUp, (2,6), (6,6) );
        }

        public void Draw(CCGraphics g, Color theColor, string theString, float theLeading, float theSpace)
        {
	        var myX = 0f;
	        var myY = 0f;

	        var myIndex = 0;
	        var myTextBuffer = theString.ToCharArray();
		
			g.PushMatrix();
			g.Scale(8f / 12f,1);
			
	        g.BeginShape(CCDrawMode.LINES);
	        g.Color(theColor);
	        
	        while (myIndex < myTextBuffer.Length)
	        {
		        var c = myTextBuffer[myIndex];
		        if (c == '\n') {
			        myY -= theLeading;
			        myX = 0;
			        myIndex++;
			        continue;
		        }
		        
		        if (c != ' ' && _charSet[c] != null) {
			        _charSet[c].Draw(g, myX, myY);
		        }
		        myX += 1 + theSpace;
				
		        myIndex++;
		        
	        }
	        
	        g.EndShape();
	        g.PopMatrix();
        }
    }
}