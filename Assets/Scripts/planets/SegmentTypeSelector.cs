using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

public class SegmentTypeSelector : MonoBehaviour
{
    [SerializeField] private bool[] m_corners = new bool[3];
    [SerializeField] private bool[] m_sides = new bool[3];

    private static bool[] ms_corners = new bool[3];
    private static bool[] ms_sides = new bool[3];

    public bool test = false;

    // Update is called once per frame
    void Update()
    {   if(test)
        {
            if (test)
            {
                test = false;

            }
        }


        ms_corners = m_corners;
        ms_sides = m_sides;
    }

    public static void FindSegmentType(IcoSegment segment, out IcoSegment.SegmentType outType, out int outOrientation)
    {
        // Reset
        for (int i = 0; i < 3; ++i)
        {
            ms_corners[i] = false;
            ms_sides[i] = false;
        }

        for (int i = 0; i < 3; ++i)
        {
            if (segment.neighbours[i].heightLevel < 
                segment.heightLevel)
            {
                ms_sides[i] = true;
            }
            if (!FullLoopLeft(segment, i))
            {
                ms_corners[i] = true;
            }
        }

        EdgesSidesToType(segment, ms_corners, ms_sides, out outType, out outOrientation);
    }

    private static void EdgesSidesToType(IcoSegment segment, bool[] corners, bool[] sides, out IcoSegment.SegmentType outType, out int outOrientation)
    {
        int[] minCorner = new int[3];
        int[] maxCorner = new int[3];

        // level
        int nbLower = 0;
        int nbHigher = 0;
        int nbLowerPlus = 0;
        int nbHigherPlus = 0;
        int nbEqual = 0;

        // Shape
        int nbCorners = 0;
        int nbSides = 0;

        // Finding
        int corner = 0;
        int side = 0;
        int noCorner = 0;
        int noSide = 0;
        int lowest = 0;
        int highest = 0;

        for (int i = 0; i < 3; ++i)
        {
            minCorner[i] = MinHeightAtCorner(segment, i);
            maxCorner[i] = MaxHeightAtCorner(segment, i);

            // Corners
            if (minCorner[i] < segment.heightLevel)
            {
                ++nbLower;
            }
            if(maxCorner[i] > segment.heightLevel)
            {
                ++nbHigher;
            }
            if (minCorner[i] + 1 < segment.heightLevel)
            {
                ++nbLowerPlus;
            }
            if (maxCorner[i] - 1 > segment.heightLevel)
            {
                ++nbHigherPlus;
            }
            if (maxCorner[i] == minCorner[i]  && minCorner[i] == segment.heightLevel)
            {
                ++nbEqual;
            }

            if(minCorner[i] < minCorner[lowest])
            {
                lowest = i;
            }
            if (minCorner[i] > minCorner[highest])
            {
                highest = i;
            }

            // Corners
            if (corners[i])
            {
                ++nbCorners;
                corner = i;
            }
            else
            {
                noCorner = i;
            }

            // Sides
            if (sides[i])
            {
                ++nbSides;
                side = i;
            }
            else
            {
                noSide = i;
            }
        }



       /* print(
            "minCorner:" + minCorner[0] + minCorner[1] + minCorner[2] +
            " maxCorner:" + maxCorner[0] + maxCorner[1] + maxCorner[2] + 
            " nbLower:" + nbLower + " nbHigher:" + nbHigher + " nbEqual:" + nbEqual + 
            " nbLowerPlus:" + nbLowerPlus + " nbHigherPlus:" + nbHigherPlus);*/

        outOrientation = 0;

        if (nbCorners == 0 && nbSides == 0)
        {
            outType = IcoSegment.SegmentType.full;
        }
        else if (nbCorners == 1 && nbSides == 0)
        {
            outType = IcoSegment.SegmentType.corner1;
            outOrientation = corner;
        }
        else if (nbCorners == 2 && nbSides == 0)
        {
            outType = IcoSegment.SegmentType.corner2;
            outOrientation = noCorner;
        }
        else if (nbCorners == 3 && nbSides == 0)
        {
            outType = IcoSegment.SegmentType.corner3;
        }
        else if (nbCorners == 3 && nbSides == 2)
        {           
            if (MinHeightAtCorner(segment, (noSide + 2) % 3) < segment.heightLevel - 1)
            {
                outOrientation = (noSide + 2) % 3;
                outType = IcoSegment.SegmentType.side2Corner;
            }
            else
            {
                outOrientation = noSide;
                outType = IcoSegment.SegmentType.side2;
            }

        }
        else if (nbCorners == 3 && nbSides == 3)
        {
            if( nbLowerPlus == 3)
            {
                outType = IcoSegment.SegmentType.side3corner3;
            }
            else if (nbLowerPlus == 2 && nbLower == 3)
            {
                outType = IcoSegment.SegmentType.side3corner2;
                outOrientation = highest;
            }
            else
            {
                outType = IcoSegment.SegmentType.side3;
            }
        }
        else if (nbSides == 1 && nbCorners > 0)
        {
            if (corners[(side + 2) % 3])
            {
                outType = IcoSegment.SegmentType.sidecorner;
                outOrientation = side;
            }
            else
            {
                outType = IcoSegment.SegmentType.side1;
                outOrientation = (side + 2) % 3;
            }
        }
        else if (nbSides == 1)
        {
            outType = IcoSegment.SegmentType.side1;
            outOrientation = noSide;
        }
        else if (nbSides == 2)
        {
            outType = IcoSegment.SegmentType.side2;
            outOrientation = noSide;
        }
        else if (nbSides == 3)
        {
            outType = IcoSegment.SegmentType.side3;
        }
        else
        {
            outType = IcoSegment.SegmentType.full;
            outOrientation = 0;
        }
    }

    public static int MinHeightAtCorner(IcoSegment segment, int corner)
    {
        int min = int.MaxValue;
        IcoSegment current = segment.neighbours[corner];
        IcoSegment previous = segment;
        while (current != segment)
        {
            min = Mathf.Min(min, current.heightLevel);
            IcoSegment tmp = current;
            current = current.LeftFrom(previous);
            previous = tmp;
        }
        return min;
    }

    public static int MaxHeightAtCorner(IcoSegment segment, int corner)
    {
        int max = int.MinValue;
        IcoSegment current = segment.neighbours[corner];
        IcoSegment previous = segment;
        while (current != segment)
        {
            max = Mathf.Max(max, current.heightLevel);
            IcoSegment tmp = current;
            current = current.LeftFrom(previous);
            previous = tmp;
        }
        return max;
    }

    public static int MaxHeightBelow(IcoSegment segment, int corner)
    {
        int max = int.MinValue;
        IcoSegment current = segment.neighbours[corner];
        IcoSegment previous = segment;
        while (current != segment)
        {
            if(current.heightLevel < segment.heightLevel)
                max = Mathf.Max(max, current.heightLevel);
            IcoSegment tmp = current;
            current = current.LeftFrom(previous);
            previous = tmp;
        }
        return max;
    }

    

    public static int[] MinHeightAtCorners( IcoSegment segment )
    {
        int[] heights = new int[3];
        for( int i = 0; i <3; ++i)
        {
            heights[i] = MinHeightAtCorner(segment, i);
        }
        return heights;
    }

    public int IndexCorrespondingleftVertice(IcoSegment segment, IcoSegment target)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (target.neighbours[i] == segment)
            {
                return (i + 1) % 3;
            }
        }
        return -1;
    }

    public static bool FullLoopLeft(IcoSegment segment,  int side)
    {
        IcoSegment current = segment.neighbours[side];
        IcoSegment previous = segment;
        while (current != segment)
        {
            if (current.heightLevel < segment.heightLevel)
            {
                return false;
            }
            IcoSegment tmp = current;
            current = current.LeftFrom(previous);
            previous = tmp;
        }
        return true;
    }

    public static bool LoopLeftBelowLevel(IcoSegment segment, int side, int level)
    {
        IcoSegment current = segment.neighbours[side];
        IcoSegment previous = segment;
        while (current != segment)
        {
            if (current.heightLevel < level)
            {
                return false;
            }
            IcoSegment tmp = current;
            current = current.LeftFrom(previous);
            previous = tmp;
        }
        return true;
    }

}
