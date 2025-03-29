using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
public class TestLandPlaceable
{

    /// <summary>
    /// Test that placing a landPlaceable on land works.
    /// </summary>
    [Test]
    public void TestPlaceOnLand()
    {
        //If the player has enough money:
        //  players money reduces by the placeables cost
        //  If there is enough power to run the building
        //      income and pollution should be updated
        //If the player doesn’t have enough money:
        //  an exception should be raised

    }

    /// <summary>
    /// Test that placing a landPlaceable on water doesnt work.
    /// </summary>
    [Test]
    public void TestPlaceOnWater()
    {
        //An exception should be raised
    }

    /// <summary>
    /// Test that placing a landPlaceable on an invalid type of tile doesnt work.
    /// E.g. placing a farm on a barren tile.
    /// </summary>
    [Test]
    public void TestPlaceOnInvalidTile()
    {
        //For example, a farm on a barren tile
        //An exception should be raised
    }


    /// <summary>
    /// Test that increasing the power level of a landPlaceable causes the expected results.
    /// </summary>
    [Test]
    public void TestIncreasePowerLevel()
    {
        //If there is enough global power:
        //  Update building pollution and income values
        //  Adjust global income and pollution levels accordingly
        //If there isnt enough global power:
        //  An exception should be raised

    }

    /// <summary>
    /// Test that decreasing the power level of a landPlaceable causes the expected results.
    /// </summary>
    [Test]
    public void TestDecreasePowerLevel()
    {
        //Update building pollution and income values
        //Adjust global income and pollution levels accordingly
    }
}
