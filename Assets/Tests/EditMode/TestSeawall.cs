using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestSeawall
{
    /// <summary>
    /// Test that building a seawall before the water level rises will block the tile from flooding
    /// </summary>
    [Test]
    public void TestBuildBeforeWaterRise()
    {
        //Water level should stop at the seawall

        //Create a central tile at elevation 1

        //Fill adjacent tiles with elevation 2 tiles

        //Set left - most tile to elevation 1

        //Build Seawall along center tiles left-most edge

        //Raise water level to 1

    }

    /// <summary>
    /// Test that building a seawall after water rises has no impact on the water level on either side of the seawall
    /// </summary>
    [Test]
    public void TestBuildAfterWaterRise()
    {
        //Water level on either side of the seawall should not change
    }
}
