using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpecification : MonoBehaviour
{
    public class LevelSpec
    {
        public float mapDimension;
        public float totalTime;
        public int requiredNumberOfPlanets;
    }

    public static LevelSpec[] levelSpecs;

    public static void Initialize() {
        levelSpecs = new LevelSpec[GenericButtonListener.numberOfLevels];   // 9 la momentul actual

        for (int i = 0; i < GenericButtonListener.numberOfLevels; i++)
        {
            levelSpecs[i] = new LevelSpec();
        }

        levelSpecs[0].mapDimension = 10.0f;
        levelSpecs[0].totalTime = 30.0f;
        levelSpecs[0].requiredNumberOfPlanets = 5;

        levelSpecs[1].mapDimension = 20.0f;
        levelSpecs[1].totalTime = 30.0f;
        levelSpecs[1].requiredNumberOfPlanets = 10;

        levelSpecs[2].mapDimension = 30.0f;
        levelSpecs[2].totalTime = 40.0f;
        levelSpecs[2].requiredNumberOfPlanets = 15;

        levelSpecs[3].mapDimension = 15.0f;
        levelSpecs[3].totalTime = 15.0f;
        levelSpecs[3].requiredNumberOfPlanets = 7;

        levelSpecs[4].mapDimension = 20.0f;
        levelSpecs[4].totalTime = 10.0f;
        levelSpecs[4].requiredNumberOfPlanets = 5;

        levelSpecs[5].mapDimension = 30.0f;
        levelSpecs[5].totalTime = 30.0f;
        levelSpecs[5].requiredNumberOfPlanets = 20;

        levelSpecs[6].mapDimension = 20.0f;
        levelSpecs[6].totalTime = 20.0f;
        levelSpecs[6].requiredNumberOfPlanets = 15;

        levelSpecs[7].mapDimension = 15.0f;
        levelSpecs[7].totalTime = 60.0f;
        levelSpecs[7].requiredNumberOfPlanets = 20;

        levelSpecs[8].mapDimension = 20.0f;
        levelSpecs[8].totalTime = 40.0f;
        levelSpecs[8].requiredNumberOfPlanets = 30;

        changeToLevel(0);
    }

    public static void changeToLevel(int level)
    {
        LevelSpec currentLevel = levelSpecs[level];
        LevelParameters.currentLevel = level;
        LevelParameters.mapDimension = currentLevel.mapDimension;
        LevelParameters.totalTime = currentLevel.totalTime;
        LevelParameters.requiredNumberOfPlanets = currentLevel.requiredNumberOfPlanets;
        LevelParameters.numberOfPlanetsLeft = LevelParameters.requiredNumberOfPlanets;

        LevelParameters.gameWon = false;    // Reset to the gameWon status from the last game.

        LoadMeteors.r = LevelParameters.mapDimension;
        LoadMeteors.R = LevelParameters.mapDimension * 1.25f;
        LoadMeteors.height = LevelParameters.mapDimension * 0.25f;
    }
}
