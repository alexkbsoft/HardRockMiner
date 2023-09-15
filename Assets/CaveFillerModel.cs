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

	public CaveFillerModel(CavePatternSO cavePattern, int segmentBlockCount)
	{
		_sizeX = cavePattern.SizeX * segmentBlockCount;
		_sizeY = cavePattern.SizeY * segmentBlockCount;
		_caveContent = new int[_sizeX, _sizeY];
		_patternSO = cavePattern;
		_segmentBlockCount = segmentBlockCount;
	}

	public int[,] Init(int[,] caveInfo, BiomeSO biome)
	{
		Random r = new Random();
		r.Next(0, _sizeX);
		_caveContent = caveInfo;

		if (_patternSO.Deposits != null)
		{
			foreach (CavePatternSO.Deposit deposit in _patternSO.Deposits)
			{
				PlaceRoundDeposit(deposit.BlockIndex, deposit.Size);
			}
		}

		if (_patternSO.Wormholes != null)
		{
			foreach (CavePatternSO.Deposit wormhole in _patternSO.Wormholes)
			{
				PlaceWormHole(wormhole.BlockIndex, wormhole.Size);
			}
		}

		if (_patternSO.DepositsLayer2 != null)
		{
			foreach (CavePatternSO.Deposit deposit in _patternSO.DepositsLayer2)
			{
				PlaceRoundDeposit(deposit.BlockIndex, deposit.Size,0,0,0,true);
			}
		}

		for (int i = 0; i < _patternSO.SpawnersCount; i++)
		{
			PlaceRoundDeposit(0, 4, 0, 0, 10);

		}

		//располагаем украшения
		if (_patternSO.Decorations != null)
		{
			foreach (CavePatternSO.Decoration decoration in _patternSO.Decorations)
			{
				int[] decorationBlocks = biome.Decorations.GetValueOrDefault(decoration.Type);				
				if (decorationBlocks == null|| decorationBlocks .Length == 0)
				{
					continue;
				};
				for (int i = 0; i < decoration.Count; i++)
				{
					Point currentPoint = new Point();
					int decorationBlockIndex = decorationBlocks[r.Next(0, decorationBlocks.Length)];
					currentPoint.x = r.Next(0, _sizeX);
					currentPoint.y = r.Next(0, _sizeY);
					int attemps = 0;
					while (_caveContent[currentPoint.x, currentPoint.y] <= 0 && attemps < 100)
					{
						currentPoint.x = r.Next(0, _sizeX);
						currentPoint.y = r.Next(0, _sizeY);
						attemps++;
					}
					if (attemps >= 100) continue;
					_caveContent[currentPoint.x, currentPoint.y] = decorationBlockIndex;
				}
			}
		}

		//разполагаем комнаты
		if (_patternSO.CaveRooms != null)
		{
			foreach (CavePatternSO.FixedRoom room in _patternSO.CaveRooms)
			{
				if (room.Positions == null)
				{
					room.Room.Init(0);
				}
				else
				{
					int roomPositionVariant = r.Next(0, room.Positions.Length);
					room.Room.Init(room.Positions[roomPositionVariant].rotation);
					PlaceRoom(room.Room, room.Positions[roomPositionVariant].x, room.Positions[roomPositionVariant].y);
				}

			}
		}


		//расчищаем зону высадки игрока
		PlaceRoundDeposit(0, 4, Convert.ToInt32(_patternSO.StartPosition.x), Convert.ToInt32(_patternSO.StartPosition.y));

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

	private void PlaceRoom(CaveRoom room, int PositionX, int PositionY)
	{
		Random r = new Random();
		int[,] currentRoom = room.GetRoom();
		int roomSizeX = currentRoom.GetLength(1);
		int roomSizeY = currentRoom.GetLength(0);
		Point currentPoint = new Point();
		if (PositionX <= 0|| PositionY <= 0)
		{
			currentPoint.x = r.Next(0, _sizeX);
			currentPoint.y = r.Next(0, _sizeY);
			int attempts = 0;
			while (_caveContent[currentPoint.x, currentPoint.y] <= 0 && attempts < 100)
			{
				currentPoint.x = r.Next(0, _sizeX);
				currentPoint.y = r.Next(0, _sizeY);
				attempts++;
			}
			if (attempts >= 100)
			{
				return;
			}
		}
		else
		{
			currentPoint.x = PositionX;
			currentPoint.y = PositionY;
		}
		for (int y = 0; y < currentRoom.GetLength(1); y++)
		{
			for (int x = 0; x < currentRoom.GetLength(0); x++)
			{
				if(currentPoint.x + x<_sizeX&& currentPoint.y + y < _sizeY)
				{
					_caveContent[currentPoint.x + x, currentPoint.y + y] = currentRoom[x, y];
				}
			}
		}
	}

	private void PlaceWormHole(int cubeType, int steps)
	{
		Random r = new Random();
		Point currentPoint = new Point();
		currentPoint.x = r.Next(0, _sizeX);
		currentPoint.y = r.Next(0, _sizeY);
		int currentStep = r.Next(0, 3);
		int attempts = 0;
		int prevStep = -1;
		int sameSteps = 0;
		int maxAttempts = 10;
		int maxSameSteps = 3;

		Point prevPoint = new Point();
		Point nextPoint = new Point();
		Point step = new Point();
		while (_caveContent[currentPoint.x, currentPoint.y] <= 0)
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
				currentStep = r.Next(0, 4);
				if (currentStep == prevStep) sameSteps++; else sameSteps = 0;
				if (sameSteps >= maxSameSteps) continue;
				switch (currentStep)
				{
					case 0: //right
							if (prevStep==1)
							{
								attempts++;
								continue;
							}
							nextPoint.x = currentPoint.x+1;
							nextPoint.y = currentPoint.y; 
							break;
					case 1: //left
							if (prevStep == 0)
							{
								attempts++;
								continue;
							}
							nextPoint.x = currentPoint.x-1;
							nextPoint.y = currentPoint.y;
							break;
					case 2:
							if (prevStep == 3)
							{
								attempts++;
								continue;
							}
							nextPoint.x = currentPoint.x;
							nextPoint.y = currentPoint.y+1;
							break;
					case 3:
							if (prevStep == 2)
							{
								attempts++;
								continue;
							}
							nextPoint.x = currentPoint.x;
							nextPoint.y = currentPoint.y-1;
							break;
					default: break ;
				}
				if (nextPoint.x >= _sizeX || nextPoint.y >= _sizeY || nextPoint.x < 0 || nextPoint.y < 0) 
				{
					nextStepValid = false;
				}
				else if(_caveContent[nextPoint.x, nextPoint.y] <= 0 || (nextPoint.x == prevPoint.x && nextPoint.y == prevPoint.y)|| _caveContent[nextPoint.x, nextPoint.y] == cubeType)
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
				switch (currentStep)
				{
					case 0: //right
						if(CheckValidBlock(currentPoint.x, currentPoint.y+1)) _caveContent[currentPoint.x, currentPoint.y+1] = cubeType;
						else if(CheckValidBlock(currentPoint.x, currentPoint.y - 1)) _caveContent[currentPoint.x, currentPoint.y-1] = cubeType;
						break;
					case 1: //left
						if (CheckValidBlock(currentPoint.x, currentPoint.y - 1)) _caveContent[currentPoint.x, currentPoint.y - 1] = cubeType;
						else if (CheckValidBlock(currentPoint.x, currentPoint.y + 1)) _caveContent[currentPoint.x, currentPoint.y + 1] = cubeType;
						break;
					case 2: //up
						if (CheckValidBlock(currentPoint.x+1, currentPoint.y)) _caveContent[currentPoint.x+1, currentPoint.y] = cubeType;
						else if (CheckValidBlock(currentPoint.x-1, currentPoint.y)) _caveContent[currentPoint.x-1, currentPoint.y] = cubeType;
						break;
					case 3: //down
						if (CheckValidBlock(currentPoint.x-1, currentPoint.y)) _caveContent[currentPoint.x-1, currentPoint.y] = cubeType;
						else if (CheckValidBlock(currentPoint.x+1, currentPoint.y)) _caveContent[currentPoint.x+1, currentPoint.y] = cubeType;
						break;
					default: break;
				}
				//Debug.WriteLine("Worm step to " + currentPoint.x + ":" + currentPoint.y);
			}	
		}
	}

	private bool CheckValidBlock(int x, int y)
	{
		if (x >= _sizeX || y >= _sizeY || x < 0 || y < 0)
		{
			return false;
		}
		else if (_caveContent[x, y] <= 0)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	private void PlaceRoundDeposit(int cubeType, int radius, int fixedX = 0, int fixedY = 0, int centerCubeType = 0, bool placeInFreeSpace = false)
	{
		Random r = new Random();
		int depositX = r.Next(0, _sizeX);
		int depositY = r.Next(0, _sizeY);
		int attepts = 0;
		if (fixedX == 0 && fixedY == 0)
		{
			while (_caveContent[depositX, depositY] == 0 && attepts < 100)
			{
				depositX = r.Next(0, _sizeX);
				depositY = r.Next(0, _sizeY);
				attepts++;
			}
		}
		else
		{
			depositX = fixedX;
			depositY = fixedY;
		}
		
		if (attepts >= 100) return;
		
		for (int y = -radius; y < radius; y++)
		{
			for (int x = -radius; x < radius ; x++)
			{
				if (IsPointsOfCircle(depositX, depositY, depositX + x, depositY + y, radius))
				{
					if (depositX + x >= 0 && depositX + x < _sizeX && depositY + y >= 0 && depositY + y < _sizeY)
					{ 
						if (placeInFreeSpace)
						{
							if (_caveContent[depositX + x, depositY + y] >= 0)
								_caveContent[depositX + x, depositY + y] = cubeType;
						}
						else
						{
							if (_caveContent[depositX + x, depositY + y] > 0)
								_caveContent[depositX + x, depositY + y] = cubeType;
						}
					}
				}
			}
		}
		if (centerCubeType != 0)
		{
			_caveContent[depositX, depositY] = centerCubeType;
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
