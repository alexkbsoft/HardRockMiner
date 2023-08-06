using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class CaveFillerModel
{
	private int[,] _caveContent;
	private CavePatternSO _patternSO;
	private int _sizeX;
	private int _sizeY;
	private int _segmentsX;
	private int _segmentsY;
	private int _segmentBlockCount;

	public CaveFillerModel(CavePatternSO cavePattern, int segmentBlockCount = 10)
	{
		_sizeX = Convert.ToInt32(cavePattern.Size.x) * segmentBlockCount;
		_sizeY = Convert.ToInt32(cavePattern.Size.y) * segmentBlockCount;
		_caveContent = new int[_sizeX, _sizeY];
		_patternSO = cavePattern;
		_segmentBlockCount = segmentBlockCount;
	}

	public int[,] Init(int[,] caveInfo)
	{
		Random r = new Random();
		r.Next(0, _sizeX);
		_caveContent = caveInfo;
		PlaceRoundDeposit(2, r.Next(5, 8));
		PlaceRoundDeposit(2, r.Next(5, 8));
		PlaceRoundDeposit(2, r.Next(5, 8));
		//PlaceRoundDeposit(3, r.Next(4, 6));
		//PlaceRoundDeposit(3, r.Next(4, 6));
		//PlaceRoundDeposit(4, r.Next(2, 4));

		//placing some bubbles

		PlaceRoundDeposit(0, r.Next(3, 5));
		PlaceRoundDeposit(0, r.Next(3, 5));
		PlaceRoundDeposit(0, r.Next(2, 4));
		PlaceRoundDeposit(0, r.Next(2, 4));

		PlaceWormHole(0, 20);
		PlaceWormHole(0, 25);
		PlaceWormHole(0, 30);

		return _caveContent;
	}

	public string Fill()
	{
		//Test fill with string data
		string cave = "";
		for (int x = 0; x < _sizeX; x++)
		{
			string caveLine = "";
			for (int y = 0; y < _sizeY; y++)
			{
				caveLine += _caveContent[x, y].ToString() + " ";
			}
			cave += caveLine + "\r\n";
		}
		return cave;
	}

	public int[,] GetCaveMap()
    {
		return _caveContent;
    }

	private void PlaceWormHole(int cubeType, int steps)
	{
		Random r = new Random();
		Point currentPoint = new Point();
		currentPoint.x = r.Next(0, _sizeX);
		currentPoint.y = r.Next(0, _sizeY);
		int currentStep = r.Next(0, 3);
		int attempts = 0;
		int maxAttempts = 5;
		Point prevPoint = new Point();
		Point nextPoint = new Point();
		Point step = new Point();
		while (_caveContent[currentPoint.x, currentPoint.y] == 0)
		{
			currentPoint.x = r.Next(0, _sizeX);
			currentPoint.y = r.Next(0, _sizeY);
		}
		// 0- go right
		// 1- go left
		// 2- go up
		// 3- go down
		//Debug.WriteLine("Worm start at " + currentPoint.x + ":" + currentPoint.y);

		_caveContent[currentPoint.x, currentPoint.y] = cubeType;
		nextPoint = currentPoint;
		for (int i = 0; i < steps; i++)
		{
			attempts = 0;
			bool nextStepValid=false;
			while(!nextStepValid && attempts < maxAttempts)
			{
				currentStep = r.Next(0, 3);
				switch (currentStep)
				{
					case 0: nextPoint.x = currentPoint.x+1;
							nextPoint.y = currentPoint.y; 
							break;
					case 1:
							nextPoint.x = currentPoint.x-1;
							nextPoint.y = currentPoint.y;
							break;
					case 2:
							nextPoint.x = currentPoint.x;
							nextPoint.y = currentPoint.y+1;
							break;
					case 3:
							nextPoint.x = currentPoint.x;
							nextPoint.y = currentPoint.y-1;
							break;
					default: break ;
				}
				if (nextPoint.x >= _sizeX || nextPoint.y >= _sizeY || nextPoint.x < 0 || nextPoint.y < 0) 
				{
					nextStepValid = false;
				}
				else if(_caveContent[nextPoint.x, nextPoint.y] == 0 || (nextPoint.x == prevPoint.x && nextPoint.y == prevPoint.y)|| _caveContent[nextPoint.x, nextPoint.y] == cubeType)
				{
					nextStepValid = false;
				}
				else
				{
					nextStepValid = true;
				}

				attempts++;
			}

			if (attempts >= maxAttempts)
			{
				break;
			}
			else
			{
				prevPoint = currentPoint;
				currentPoint = nextPoint;
				_caveContent[currentPoint.x, currentPoint.y] = cubeType;
				//Debug.WriteLine("Worm step to " + currentPoint.x + ":" + currentPoint.y);
			}	
		}
	}

	private void PlaceRoundDeposit(int cubeType, int radius)
	{
		Random r = new Random();
		int depositX = r.Next(0, _sizeX);
		int depositY = r.Next(0, _sizeY);
		while (_caveContent[depositX, depositY] == 0)
		{
			depositX = r.Next(0, _sizeX);
			depositY = r.Next(0, _sizeY);
		}
		for (int y = -radius; y < radius; y++)
		{
			for (int x = -radius; x < radius ; x++)
			{
				if (IsPointsOfCircle(depositX, depositY, depositX+x, depositY+y, radius))
				{
					if(depositX + x > 0 && depositX + x<_sizeX && depositY + y>0 && depositY + y < _sizeY)
						if (_caveContent[depositX + x, depositY + y]!= 0)
							_caveContent[depositX + x, depositY + y] = cubeType;
				}
			}
		}
	}

	private bool IsPointsOfCircle(int xCenter, int yCenter, int x, int y, int radius)
	{
		int dx = xCenter - x;
		int dy = yCenter - y;
		return (dx * dx + dy * dy) <= radius * radius;
	}

	public struct Point
	{
		public int x;
		public int y;
	}
}
