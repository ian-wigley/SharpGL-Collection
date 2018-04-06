using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MetaBallsEnviro
{
    /// <summary>
    /// The main form class.
    /// </summary>
    /// 

    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        /// 

        /// <summary>
        /// Metaballs array data
        /// </summary>
        /// 
        #region

        int[] EdgeTable = {
            0x0  , 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
            0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
            0x190, 0x99 , 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
            0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
            0x230, 0x339, 0x33 , 0x13a, 0x636, 0x73f, 0x435, 0x53c,
            0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
            0x3a0, 0x2a9, 0x1a3, 0xaa , 0x7a6, 0x6af, 0x5a5, 0x4ac,
            0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
            0x460, 0x569, 0x663, 0x76a, 0x66 , 0x16f, 0x265, 0x36c,
            0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
            0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff , 0x3f5, 0x2fc,
            0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
            0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55 , 0x15c,
            0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
            0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc ,
            0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
            0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
            0xcc , 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
            0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
            0x15c, 0x55 , 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
            0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
            0x2fc, 0x3f5, 0xff , 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
            0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
            0x36c, 0x265, 0x16f, 0x66 , 0x76a, 0x663, 0x569, 0x460,
            0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
            0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa , 0x1a3, 0x2a9, 0x3a0,
            0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
            0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33 , 0x339, 0x230,
            0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
            0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99 , 0x190,
            0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
            0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0};

        int[,] TriangleTable = {
    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
    {3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
    {3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
    {3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
    {9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
    {9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
    {2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
    {8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
    {9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
    {4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
    {3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
    {1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
    {4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
    {4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
    {9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
    {5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
    {2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
    {9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
    {0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
    {2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
    {10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
    {4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
    {5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
    {5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
    {9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
    {0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
    {1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
    {10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
    {8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
    {2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
    {7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
    {9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
    {2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
    {11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
    {9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
    {5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
    {11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
    {11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
    {1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
    {9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
    {5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
    {2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
    {0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
    {5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
    {6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
    {3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
    {6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
    {5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
    {1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
    {10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
    {6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
    {8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
    {7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
    {3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
    {5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
    {0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
    {9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
    {8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
    {5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
    {0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
    {6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
    {10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
    {10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
    {8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
    {1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
    {3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
    {0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
    {10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
    {3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
    {6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
    {9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
    {8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
    {3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
    {6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
    {0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
    {10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
    {10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
    {2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
    {7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
    {7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
    {2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
    {1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
    {11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
    {8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
    {0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
    {7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
    {10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
    {2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
    {6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
    {7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
    {2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
    {1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
    {10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
    {10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
    {0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
    {7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
    {6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
    {8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
    {9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
    {6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
    {4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
    {10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
    {8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
    {0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
    {1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
    {8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
    {10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
    {4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
    {10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
    {5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
    {11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
    {9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
    {6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
    {7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
    {3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
    {7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
    {9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
    {3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
    {6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
    {9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
    {1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
    {4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
    {7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
    {6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
    {3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
    {0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
    {6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
    {0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
    {11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
    {6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
    {5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
    {9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
    {1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
    {1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
    {10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
    {0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
    {5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
    {10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
    {11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
    {9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
    {7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
    {2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
    {8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
    {9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
    {9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
    {1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
    {9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
    {9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
    {5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
    {0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
    {10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
    {2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
    {0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
    {0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
    {9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
    {5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
    {3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
    {5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
    {8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
    {0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
    {9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
    {0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
    {1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
    {3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
    {4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
    {9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
    {11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
    {11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
    {2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
    {9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
    {3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
    {1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
    {4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
    {4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
    {0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
    {3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
    {3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
    {0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
    {9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
    {1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1} };

        #endregion


        float Angle = 0;
        long LastTime = 0;
        DateTime startTime, endTime;
        int elapsedMillisecs;

        Texture texture1 = new Texture();
        Texture texture2 = new Texture();
        private uint BackTex = 0;
        private uint TubeTex = 0;

        private List<float> valueTest = new List<float>();

        public struct TGLCoord
        {
            public float X;
            public float Y;
            public float Z;
        }

        public struct TMetaBall
        {
            public float Radius;
            public float X;
            public float Y;
            public float Z;
        }

        public struct TGridPoint
        {
            public TGLCoord Pos;
            public TGLCoord Normal;
            // Result of the metaball equations at this point
            public float Value;
        }

        public struct TGridCube
        {
            public TGridCube(int count)
                : this()
            {
                GridPoint = new TGridPoint[8];
            }
            public TGridPoint[] GridPoint;
        }

        // User vaiables
        bool wireframe;
        bool SmoothShading;
        bool Textured;
        int GridSize;
        // Number of triangles by metaball tesselation.
        int TessTriangles;
        TMetaBall[] MetaBall = new TMetaBall[4];

        // for this demo set max gridsize = 50
        TGridPoint[, ,] Grid = new TGridPoint[50, 50, 50];
        TGridCube[, ,] Cubes = new TGridCube[49, 49, 49];
        TGLCoord[] VertList = new TGLCoord[12];
        TGLCoord[] Norm = new TGLCoord[12];

        public SharpGLForm()
        {
            startTime = DateTime.Now;
            InitializeComponent();
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenderEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();

            Angle = (elapsedMillisecs / 32.0f);

            endTime = DateTime.Now;
            elapsedMillisecs = (int)((TimeSpan)(endTime - startTime)).TotalMilliseconds / 2;
            LastTime = elapsedMillisecs;

            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(0, 0, -2);

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.LineWidth(3);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(-0.32f, -0.28f, 0.0f);
            gl.Vertex(0.32f, -0.28f, 0.0f);
            gl.Vertex(0.32f, 0.28f, 0.0f);
            gl.Vertex(-0.32f, 0.28f, 0.0f);
            gl.End();
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            glDraw(gl);
        }


        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Initialise & populate array with 0's
            for (int i = 0; i < 49; i++)
            {
                for (int ii = 0; ii < 49; ii++)
                {
                    for (int iii = 0; iii < 49; iii++)
                    {
                        Cubes[i, ii, iii].GridPoint = new TGridPoint[8];
                        for (int j = 0; j < 8; j++)
                        {
                            Cubes[i, ii, iii].GridPoint[j].Normal.X = 0;
                            Cubes[i, ii, iii].GridPoint[j].Normal.Y = 0;
                            Cubes[i, ii, iii].GridPoint[j].Normal.Z = 0;
                        }
                    }
                }
            }

            glInit(gl);

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
        }

        //{------------------------------------------------------------------}
        //{  Initialise OpenGL                                               }
        //{------------------------------------------------------------------}
        internal void glInit(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); 	    // Black Background
            gl.ShadeModel(OpenGL.GL_SMOOTH);            // Enables Smooth Color Shading
            gl.Enable(OpenGL.GL_DEPTH_TEST);            // Enable Depth Buffer
            gl.DepthFunc(OpenGL.GL_LESS);		        // The Type Of Depth Test To Do

            gl.Hint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);   //Realy Nice perspective calculations
            gl.Enable(OpenGL.GL_TEXTURE_2D);                     // Enable Texture Mapping


            //  Create our texture object from a file. This creates the texture for OpenGL.
            texture1.Create(gl, "chrome.bmp");
            texture2.Create(gl, "background.bmp");

            // Set up environment mapping
            gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
            gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
            gl.TexGen(OpenGL.GL_S, OpenGL.GL_SPHERE_MAP, 0);
            gl.TexGen(OpenGL.GL_T, OpenGL.GL_SPHERE_MAP, 0);

            gl.Enable(OpenGL.GL_NORMALIZE);

            // initialise the metaball size and positions
            MetaBall[1].Radius = 0.3f;
            MetaBall[1].X = 0;
            MetaBall[1].Y = 0;
            MetaBall[1].Z = 0;

            MetaBall[2].Radius = 0.22f;
            MetaBall[2].X = 0;
            MetaBall[2].Y = 0;
            MetaBall[2].Z = 0;

            MetaBall[3].Radius = 0.25f;
            MetaBall[3].X = 0;
            MetaBall[3].Y = 0;
            MetaBall[3].Z = 0;

            Textured = true;
            SmoothShading = true;
            wireframe = false;
            GridSize = 26;
            InitGrid();
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(45.0f, (double)Width / (double)Height, 1.0, 100.0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        internal void InitGrid()
        {
            int cx = 0, cy = 0, cz = 0;
            // Create the grid positions
            for (cx = 0; cx < GridSize + 1; cx++)
            {
                for (cy = 0; cy < GridSize + 1; cy++)
                {
                    for (cz = 0; cz < GridSize + 1; cz++)
                    {
                        Grid[cx, cy, cz].Pos.X = (2.0f * cx / GridSize) - 1.0f;   // grid from -1 to 1
                        Grid[cx, cy, cz].Pos.Y = (2.0f * cy / GridSize) - 1.0f;   // grid from -1 to 1
                        Grid[cx, cy, cz].Pos.Z = 1.0f - 2.0f * cz / GridSize;     // grid from -1 to 1
                    }
                }
            }
            // Create the cubes. Each cube points to 8 grid points
            for (cx = 0; cx < GridSize - 1; cx++)
            {
                for (cy = 0; cy < GridSize - 1; cy++)
                {
                    for (cz = 0; cz < GridSize - 1; cz++)
                    {
                        Cubes[cx, cy, cz].GridPoint[0] = @Grid[cx, cy, cz];
                        Cubes[cx, cy, cz].GridPoint[1] = @Grid[cx + 1, cy, cz];
                        Cubes[cx, cy, cz].GridPoint[2] = @Grid[cx + 1, cy, cz + 1];
                        Cubes[cx, cy, cz].GridPoint[3] = @Grid[cx, cy, cz + 1];
                        Cubes[cx, cy, cz].GridPoint[4] = @Grid[cx, cy + 1, cz];
                        Cubes[cx, cy, cz].GridPoint[5] = @Grid[cx + 1, cy + 1, cz];
                        Cubes[cx, cy, cz].GridPoint[6] = @Grid[cx + 1, cy + 1, cz + 1];
                        Cubes[cx, cy, cz].GridPoint[7] = @Grid[cx, cy + 1, cz + 1];
                    }
                }
            }
        }

        //{----------------------------------------------------------}
        //{  Interpolate the position where an metaballs intersects  }
        //{  the line betweenthe two coordicates, C1 and C2          }
        //{----------------------------------------------------------}
        internal void Interpolate(TGridPoint C1, TGridPoint C2, out TGLCoord CResult, out TGLCoord Norm)
        {
            float mu = 0;
            if (Math.Abs(C1.Value) == 1)
            {
                CResult = C1.Pos;
                Norm = C1.Normal;
            }
            else
                if (Math.Abs(C2.Value) == 1)
                {
                    CResult = C2.Pos;
                    Norm = C2.Normal;
                }
                else
                    if (C1.Value == C2.Value)
                    {
                        CResult = C1.Pos;
                        Norm = C1.Normal;
                    }
                    else
                    {
                        mu = (1 - C1.Value) / (C2.Value - C1.Value);
                        CResult.X = C1.Pos.X + mu * (C2.Pos.X - C1.Pos.X);
                        CResult.Y = C1.Pos.Y + mu * (C2.Pos.Y - C1.Pos.Y);
                        CResult.Z = C1.Pos.Z + mu * (C2.Pos.Z - C1.Pos.Z);

                        Norm.X = C1.Normal.X + (C2.Normal.X - C1.Normal.X) * mu;
                        Norm.Y = C1.Normal.Y + (C2.Normal.Y - C1.Normal.Y) * mu;
                        Norm.Z = C1.Normal.Z + (C2.Normal.Z - C1.Normal.Z) * mu;
                    }
        }

        //{------------------------------------------------------------}
        //{  Calculate the triangles required to draw a Cube.          }
        //{  Draws the triangles that makes up a Cube                  }
        //{------------------------------------------------------------}
        internal void CreateCubeTriangles(OpenGL gl, TGridCube GridCube)
        {
            int CubeIndex = 0;
            // Determine the index into the edge table which tells
            // us which vertices are inside/outside the metaballs

            CubeIndex = 0;
            if (GridCube.GridPoint[0].Value < 1) CubeIndex |= 1;// CubeIndex | 1;
            if (GridCube.GridPoint[1].Value < 1) CubeIndex |= 2;// CubeIndex | 2;
            if (GridCube.GridPoint[2].Value < 1) CubeIndex |= 4;// CubeIndex | 4;
            if (GridCube.GridPoint[3].Value < 1) CubeIndex |= 8;// CubeIndex | 8;
            if (GridCube.GridPoint[4].Value < 1) CubeIndex |= 16;// CubeIndex | 16;
            if (GridCube.GridPoint[5].Value < 1) CubeIndex |= 32;// CubeIndex | 32;
            if (GridCube.GridPoint[6].Value < 1) CubeIndex |= 64;// CubeIndex | 64;
            if (GridCube.GridPoint[7].Value < 1) CubeIndex |= 128;// CubeIndex | 128;

            // Check if the cube is entirely in/out of the surface
            if (EdgeTable[CubeIndex] == 0)
            {
                return;
            }

            // Find the vertices where the surface intersects the cube.
            if ((EdgeTable[CubeIndex] & 1) != 0)
            {
                Interpolate(GridCube.GridPoint[0], GridCube.GridPoint[1], out VertList[0], out Norm[0]);
            }
            if ((EdgeTable[CubeIndex] & 2) != 0)
            {
                Interpolate(GridCube.GridPoint[1], GridCube.GridPoint[2], out VertList[1], out Norm[1]);
            }
            if ((EdgeTable[CubeIndex] & 4) != 0)
            {
                Interpolate(GridCube.GridPoint[2], GridCube.GridPoint[3], out VertList[2], out Norm[2]);
            }
            if ((EdgeTable[CubeIndex] & 8) != 0)
            {
                Interpolate(GridCube.GridPoint[3], GridCube.GridPoint[0], out VertList[3], out Norm[3]);
            }
            if ((EdgeTable[CubeIndex] & 16) != 0)
            {
                Interpolate(GridCube.GridPoint[4], GridCube.GridPoint[5], out VertList[4], out Norm[4]);
            }
            if ((EdgeTable[CubeIndex] & 32) != 0)
            {
                Interpolate(GridCube.GridPoint[5], GridCube.GridPoint[6], out VertList[5], out Norm[5]);
            }
            if ((EdgeTable[CubeIndex] & 64) != 0)
            {
                Interpolate(GridCube.GridPoint[6], GridCube.GridPoint[7], out VertList[6], out Norm[6]);
            }
            if ((EdgeTable[CubeIndex] & 128) != 0)
            {
                Interpolate(GridCube.GridPoint[7], GridCube.GridPoint[4], out VertList[7], out Norm[7]);
            }
            if ((EdgeTable[CubeIndex] & 256) != 0)
            {
                Interpolate(GridCube.GridPoint[0], GridCube.GridPoint[4], out VertList[8], out Norm[8]);
            }
            if ((EdgeTable[CubeIndex] & 512) != 0)
            {
                Interpolate(GridCube.GridPoint[1], GridCube.GridPoint[5], out VertList[9], out Norm[9]);
            }
            if ((EdgeTable[CubeIndex] & 1024) != 0)
            {
                Interpolate(GridCube.GridPoint[2], GridCube.GridPoint[6], out VertList[10], out Norm[10]);
            }
            if ((EdgeTable[CubeIndex] & 2048) != 0)
            {
                Interpolate(GridCube.GridPoint[3], GridCube.GridPoint[7], out VertList[11], out Norm[11]);
            }

            // Draw the triangles for this cube
            int i = 0;
            gl.Color(1.0f, 1.0f, 1.0f);
            while (TriangleTable[CubeIndex, i] != -1)
            {
                if (Textured)
                {
                    gl.Normal(@Norm[TriangleTable[CubeIndex, i]].X, @Norm[TriangleTable[CubeIndex, i]].Y, @Norm[TriangleTable[CubeIndex, i]].Z);
                }
                else
                {
                    SetColor(gl, VertList[TriangleTable[CubeIndex, i]]);
                }
                gl.Vertex(@VertList[TriangleTable[CubeIndex, i]].X, @VertList[TriangleTable[CubeIndex, i]].Y, @VertList[TriangleTable[CubeIndex, i]].Z);

                if (Textured)
                {
                    gl.Normal(@Norm[TriangleTable[CubeIndex, i + 1]].X, @Norm[TriangleTable[CubeIndex, i + 1]].Y, @Norm[TriangleTable[CubeIndex, i + 1]].Z);
                }
                else
                {
                    SetColor(gl, VertList[TriangleTable[CubeIndex, i + 1]]);
                }
                gl.Vertex(@VertList[TriangleTable[CubeIndex, i + 1]].X, @VertList[TriangleTable[CubeIndex, i + 1]].Y, @VertList[TriangleTable[CubeIndex, i + 1]].Z);

                if (Textured)
                {
                    gl.Normal(@Norm[TriangleTable[CubeIndex, i + 2]].X, @Norm[TriangleTable[CubeIndex, i + 2]].Y, @Norm[TriangleTable[CubeIndex, i + 2]].Z);
                }
                else if (SmoothShading)
                {
                    SetColor(gl, VertList[TriangleTable[CubeIndex, i + 2]]);
                }
                gl.Vertex(@VertList[TriangleTable[CubeIndex, i + 2]].X, @VertList[TriangleTable[CubeIndex, i + 2]].Y, @VertList[TriangleTable[CubeIndex, i + 2]].Z);
                i += 3;
            }
        }

        //{------------------------------------------------------------------}
        //{  Function to draw the actual scene                               }
        //{------------------------------------------------------------------}
        internal void glDraw(OpenGL gl)
        {
            //int x = 0, y = 0, z = 0;
            int cx, cy, cz;
            float c = 0;
            int ElapsedTime = elapsedMillisecs;

            gl.Color(1.0f, 1.0f, 1.0f);
            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            // Reset The View
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -2.5f);

            gl.Disable(OpenGL.GL_TEXTURE_GEN_S);
            gl.Disable(OpenGL.GL_TEXTURE_GEN_T);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 2);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0, 0); gl.Vertex(-1.5, -1.1, 0);
            gl.TexCoord(1, 0); gl.Vertex(1.5, -1.1, 0);
            gl.TexCoord(1, 1); gl.Vertex(1.5, 1.1, 0);
            gl.TexCoord(0, 1); gl.Vertex(-1.5, 1.1, 0);
            gl.End();
            gl.Enable(OpenGL.GL_TEXTURE_GEN_S);
            gl.Enable(OpenGL.GL_TEXTURE_GEN_T);

            gl.Rotate(ElapsedTime / 30, 0, 0, 1);

            c = (float)(0.15f * Math.Cos(ElapsedTime / 600.0f));
            MetaBall[1].X = (float)(-0.3f * Math.Cos(ElapsedTime / 700.0f) - c);
            MetaBall[1].Y = (float)(0.3f * Math.Sin(ElapsedTime / 600.0f) - c);

            MetaBall[2].X = (float)(0.4f * Math.Sin(ElapsedTime / 400.0f) + c);
            MetaBall[2].Y = (float)(0.4f * Math.Cos(ElapsedTime / 400.0f) - c);

            MetaBall[3].X = (float)(-0.4f * Math.Cos(ElapsedTime / 400.0f) - 0.2f * Math.Sin(ElapsedTime / 600.0f));
            MetaBall[3].Y = (float)(0.4f * Math.Sin(ElapsedTime / 500.0f) - 0.2f * Math.Sin(ElapsedTime / 400.0f));

            TessTriangles = 0;
            for (cx = 0; cx < GridSize + 1; cx++)
            {
                for (cy = 0; cy < GridSize + 1; cy++)
                {
                    for (cz = 0; cz < GridSize + 1; cz++)
                    {
                        Grid[cx, cy, cz].Value = 0;

                        // go through all meta balls
                        for (int I = 1; I < 4; I++)
                        {
                            Grid[cx, cy, cz].Value = Grid[cx, cy, cz].Value + MetaBall[I].Radius * MetaBall[I].Radius / ((Grid[cx, cy, cz].Pos.X - MetaBall[I].X) * (Grid[cx, cy, cz].Pos.X - MetaBall[I].X) + (Grid[cx, cy, cz].Pos.Y - MetaBall[I].Y) * (Grid[cx, cy, cz].Pos.Y - MetaBall[I].Y) + (Grid[cx, cy, cz].Pos.Z - MetaBall[I].Z) * (Grid[cx, cy, cz].Pos.Z - MetaBall[I].Z));
                        }

                    }
                }
            }

            //Copy Values from Grid to the Cubes
            for (cx = 0; cx < GridSize - 1; cx++)
            {
                for (cy = 0; cy < GridSize - 1; cy++)
                {
                    for (cz = 0; cz < GridSize - 1; cz++)
                    {
                        Cubes[cx, cy, cz].GridPoint[0] = Grid[cx, cy, cz];
                        Cubes[cx, cy, cz].GridPoint[1] = Grid[cx + 1, cy, cz];
                        Cubes[cx, cy, cz].GridPoint[2] = Grid[cx + 1, cy, cz + 1];
                        Cubes[cx, cy, cz].GridPoint[3] = Grid[cx, cy, cz + 1];
                        Cubes[cx, cy, cz].GridPoint[4] = Grid[cx, cy + 1, cz];
                        Cubes[cx, cy, cz].GridPoint[5] = Grid[cx + 1, cy + 1, cz];
                        Cubes[cx, cy, cz].GridPoint[6] = Grid[cx + 1, cy + 1, cz + 1];
                        Cubes[cx, cy, cz].GridPoint[7] = Grid[cx, cy + 1, cz + 1];
                    }
                }
            }

            // Calculate normals at the grid vertices
            for (cx = 1; cx < GridSize - 1; cx++)
            {
                for (cy = 1; cy < GridSize - 1; cy++)
                {
                    for (cz = 1; cz < GridSize - 1; cz++)
                    {
                        Grid[cx, cy, cz].Normal.X = Grid[cx - 1, cy, cz].Value - Grid[cx + 1, cy, cz].Value;
                        Grid[cx, cy, cz].Normal.Y = Grid[cx, cy - 1, cz].Value - Grid[cx, cy + 1, cz].Value;
                        Grid[cx, cy, cz].Normal.Z = Grid[cx, cy, cz - 1].Value - Grid[cx, cy, cz + 1].Value;
                    }
                }
            }

            // Draw the metaballs by drawing the triangle in each cube in the grid
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 1);
            gl.Begin(OpenGL.GL_TRIANGLES);
            for (cx = 0; cx < GridSize; cx++)
            {
                for (cy = 0; cy < GridSize; cy++)
                {
                    for (cz = 0; cz < GridSize; cz++)
                    {
                        CreateCubeTriangles(gl, Cubes[cx, cy, cz]);
                    }
                }
            }
            gl.End();
        }

        internal void SetColor(OpenGL gl, TGLCoord V)
        {
            float C = 0;
            C = (float)Math.Sqrt(V.X * V.X + V.Y * V.Y + V.Z * V.Z);
            gl.Color(C, C, C + 0.1f);
        }
    }
}