extends Area2D

func _ready():
	GameManager.PlatformManager.AddArea2DSlopeStairsDown(self)
