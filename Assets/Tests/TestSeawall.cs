using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestSeawall
{
    // A Test behaves as an ordinary method
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

    [Test]
    public void TestBuildAfterWaterRise()
    {
        //Water level on either side of the seawall should not change
    }
}
