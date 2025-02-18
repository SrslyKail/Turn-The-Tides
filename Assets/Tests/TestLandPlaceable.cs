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

    [Test]
    public void TestPlaceOnWater()
    {
        //An exception should be raised
    }

    [Test]
    public void TestPlaceOnInvalidTile()
    {
        //For example, a farm on a barren tile
        //An exception should be raised
    }

    [Test]
    public void TestIncreasePowerLevel()
    {
        //If there is enough global power:
        //  Update building pollution and income values
        //  Adjust global income and pollution levels accordingly
        //If there isnt enough global power:
        //  An exception should be raised

    }

    [Test]
    public void TestDecreasePowerLevel()
    {
        //Update building pollution and income values
        //Adjust global income and pollution levels accordingly
    }
}
