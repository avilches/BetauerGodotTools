using Betauer.TestRunner;

namespace Betauer.GameTools.Tests.TileSet.Generated;

[Test]
[Only]
public class Blob47Tests : BaseBlobTests {
    // |  0|   |  2|  8| 10| 32| 34| 40| 42|128|130|136|138|160|162|168|170|
    // |   |   |  #|   |  #|   |  #|   |  #|#  |# #|#  |# #|#  |# #|#  |# #|
    // | # |   | # | # | # | # | # | # | # | # | # | # | # | # | # | # | # |
    // |   |   |   |  #|  #|#  |#  |# #|# #|   |   |  #|  #|#  |#  |# #|# #|
    [Test(Description="0 when 0,2,8,10,32,34,40,42,128,130,136,138,160,162,168,170")]
    public void TestTile0() {
        
        // Pattern where central tile with 0 mask is transformed to 0
        AssertBlob47("""
                         ...
                         .#.
                         ...
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 2 mask is transformed to 0
        AssertBlob47("""
                         ..#
                         .#.
                         ...
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 8 mask is transformed to 0
        AssertBlob47("""
                         ...
                         .#.
                         ..#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 10 mask is transformed to 0
        AssertBlob47("""
                         ..#
                         .#.
                         ..#
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 32 mask is transformed to 0
        AssertBlob47("""
                         ...
                         .#.
                         #..
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 34 mask is transformed to 0
        AssertBlob47("""
                         ..#
                         .#.
                         #..
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 40 mask is transformed to 0
        AssertBlob47("""
                         ...
                         .#.
                         #.#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 42 mask is transformed to 0
        AssertBlob47("""
                         ..#
                         .#.
                         #.#
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 128 mask is transformed to 0
        AssertBlob47("""
                         #..
                         .#.
                         ...
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 130 mask is transformed to 0
        AssertBlob47("""
                         #.#
                         .#.
                         ...
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 136 mask is transformed to 0
        AssertBlob47("""
                         #..
                         .#.
                         ..#
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 138 mask is transformed to 0
        AssertBlob47("""
                         #.#
                         .#.
                         ..#
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 160 mask is transformed to 0
        AssertBlob47("""
                         #..
                         .#.
                         #..
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 162 mask is transformed to 0
        AssertBlob47("""
                         #.#
                         .#.
                         #..
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 168 mask is transformed to 0
        AssertBlob47("""
                         #..
                         .#.
                         #.#
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 170 mask is transformed to 0
        AssertBlob47("""
                         #.#
                         .#.
                         #.#
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,    0,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
   }
    // |  1|   |  3|  9| 11| 33| 35| 41| 43|129|131|137|139|161|163|169|171|
    // | # |   | ##| # | ##| # | ##| # | ##|## |###|## |###|## |###|## |###|
    // | # |   | # | # | # | # | # | # | # | # | # | # | # | # | # | # | # |
    // |   |   |   |  #|  #|#  |#  |# #|# #|   |   |  #|  #|#  |#  |# #|# #|
    [Test(Description="1 when 1,3,9,11,33,35,41,43,129,131,137,139,161,163,169,171")]
    public void TestTile1() {
        
        // Pattern where central tile with 1 mask is transformed to 1
        AssertBlob47("""
                         .#.
                         .#.
                         ...
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 3 mask is transformed to 1
        AssertBlob47("""
                         .##
                         .#.
                         ...
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 9 mask is transformed to 1
        AssertBlob47("""
                         .#.
                         .#.
                         ..#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 11 mask is transformed to 1
        AssertBlob47("""
                         .##
                         .#.
                         ..#
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 33 mask is transformed to 1
        AssertBlob47("""
                         .#.
                         .#.
                         #..
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 35 mask is transformed to 1
        AssertBlob47("""
                         .##
                         .#.
                         #..
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 41 mask is transformed to 1
        AssertBlob47("""
                         .#.
                         .#.
                         #.#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 43 mask is transformed to 1
        AssertBlob47("""
                         .##
                         .#.
                         #.#
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 129 mask is transformed to 1
        AssertBlob47("""
                         ##.
                         .#.
                         ...
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 131 mask is transformed to 1
        AssertBlob47("""
                         ###
                         .#.
                         ...
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 137 mask is transformed to 1
        AssertBlob47("""
                         ##.
                         .#.
                         ..#
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 139 mask is transformed to 1
        AssertBlob47("""
                         ###
                         .#.
                         ..#
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,    1,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 161 mask is transformed to 1
        AssertBlob47("""
                         ##.
                         .#.
                         #..
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 163 mask is transformed to 1
        AssertBlob47("""
                         ###
                         .#.
                         #..
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 169 mask is transformed to 1
        AssertBlob47("""
                         ##.
                         .#.
                         #.#
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 171 mask is transformed to 1
        AssertBlob47("""
                         ###
                         .#.
                         #.#
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,    1,   -1, }, 
                         {   0,   -1,    0, }, 
                         });
   }
    // |  4|   |  6| 12| 14| 36| 38| 44| 46|132|134|140|142|164|166|172|174|
    // |   |   |  #|   |  #|   |  #|   |  #|#  |# #|#  |# #|#  |# #|#  |# #|
    // | ##|   | ##| ##| ##| ##| ##| ##| ##| ##| ##| ##| ##| ##| ##| ##| ##|
    // |   |   |   |  #|  #|#  |#  |# #|# #|   |   |  #|  #|#  |#  |# #|# #|
    [Test(Description="4 when 4,6,12,14,36,38,44,46,132,134,140,142,164,166,172,174")]
    public void TestTile4() {
        
        // Pattern where central tile with 4 mask is transformed to 4
        AssertBlob47("""
                         ...
                         .##
                         ...
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    4,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 6 mask is transformed to 4
        AssertBlob47("""
                         ..#
                         .##
                         ...
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,    4,   65, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 12 mask is transformed to 4
        AssertBlob47("""
                         ...
                         .##
                         ..#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    4,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 14 mask is transformed to 4
        AssertBlob47("""
                         ..#
                         .##
                         ..#
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,    4,   81, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 36 mask is transformed to 4
        AssertBlob47("""
                         ...
                         .##
                         #..
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    4,   64, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 38 mask is transformed to 4
        AssertBlob47("""
                         ..#
                         .##
                         #..
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,    4,   65, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 44 mask is transformed to 4
        AssertBlob47("""
                         ...
                         .##
                         #.#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,    4,   80, }, 
                         {   0,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 46 mask is transformed to 4
        AssertBlob47("""
                         ..#
                         .##
                         #.#
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,    4,   81, }, 
                         {   0,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 132 mask is transformed to 4
        AssertBlob47("""
                         #..
                         .##
                         ...
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    4,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 134 mask is transformed to 4
        AssertBlob47("""
                         #.#
                         .##
                         ...
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,    4,   65, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 140 mask is transformed to 4
        AssertBlob47("""
                         #..
                         .##
                         ..#
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    4,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 142 mask is transformed to 4
        AssertBlob47("""
                         #.#
                         .##
                         ..#
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,    4,   81, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 164 mask is transformed to 4
        AssertBlob47("""
                         #..
                         .##
                         #..
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    4,   64, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 166 mask is transformed to 4
        AssertBlob47("""
                         #.#
                         .##
                         #..
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,    4,   65, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 172 mask is transformed to 4
        AssertBlob47("""
                         #..
                         .##
                         #.#
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,    4,   80, }, 
                         {   0,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 174 mask is transformed to 4
        AssertBlob47("""
                         #.#
                         .##
                         #.#
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,    4,   81, }, 
                         {   0,   -1,    1, }, 
                         });
   }
    // |  5|   | 13| 37| 45|133|141|165|173|
    // | # |   | # | # | # |## |## |## |## |
    // | ##|   | ##| ##| ##| ##| ##| ##| ##|
    // |   |   |  #|#  |# #|   |  #|#  |# #|
    [Test(Description="5 when 5,13,37,45,133,141,165,173")]
    public void TestTile5() {
        
        // Pattern where central tile with 5 mask is transformed to 5
        AssertBlob47("""
                         .#.
                         .##
                         ...
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 13 mask is transformed to 5
        AssertBlob47("""
                         .#.
                         .##
                         ..#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 37 mask is transformed to 5
        AssertBlob47("""
                         .#.
                         .##
                         #..
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   64, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 45 mask is transformed to 5
        AssertBlob47("""
                         .#.
                         .##
                         #.#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   80, }, 
                         {   0,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 133 mask is transformed to 5
        AssertBlob47("""
                         ##.
                         .##
                         ...
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    5,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 141 mask is transformed to 5
        AssertBlob47("""
                         ##.
                         .##
                         ..#
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    5,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 165 mask is transformed to 5
        AssertBlob47("""
                         ##.
                         .##
                         #..
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    5,   64, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 173 mask is transformed to 5
        AssertBlob47("""
                         ##.
                         .##
                         #.#
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,    5,   80, }, 
                         {   0,   -1,    1, }, 
                         });
   }
    // |  7|   | 15| 39| 47|135|143|167|175|
    // | ##|   | ##| ##| ##|###|###|###|###|
    // | ##|   | ##| ##| ##| ##| ##| ##| ##|
    // |   |   |  #|#  |# #|   |  #|#  |# #|
    [Test(Description="7 when 7,15,39,47,135,143,167,175")]
    public void TestTile7() {
        
        // Pattern where central tile with 7 mask is transformed to 7
        AssertBlob47("""
                         .##
                         .##
                         ...
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,    7,  193, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 15 mask is transformed to 7
        AssertBlob47("""
                         .##
                         .##
                         ..#
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,    7,  209, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 39 mask is transformed to 7
        AssertBlob47("""
                         .##
                         .##
                         #..
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,    7,  193, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 47 mask is transformed to 7
        AssertBlob47("""
                         .##
                         .##
                         #.#
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,    7,  209, }, 
                         {   0,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 135 mask is transformed to 7
        AssertBlob47("""
                         ###
                         .##
                         ...
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,    7,  193, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 143 mask is transformed to 7
        AssertBlob47("""
                         ###
                         .##
                         ..#
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,    7,  209, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 167 mask is transformed to 7
        AssertBlob47("""
                         ###
                         .##
                         #..
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,    7,  193, }, 
                         {   0,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 175 mask is transformed to 7
        AssertBlob47("""
                         ###
                         .##
                         #.#
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,    7,  209, }, 
                         {   0,   -1,    1, }, 
                         });
   }
    // | 16|   | 18| 24| 26| 48| 50| 56| 58|144|146|152|154|176|178|184|186|
    // |   |   |  #|   |  #|   |  #|   |  #|#  |# #|#  |# #|#  |# #|#  |# #|
    // | # |   | # | # | # | # | # | # | # | # | # | # | # | # | # | # | # |
    // | # |   | # | ##| ##|## |## |###|###| # | # | ##| ##|## |## |###|###|
    [Test(Description="16 when 16,18,24,26,48,50,56,58,144,146,152,154,176,178,184,186")]
    public void TestTile16() {
        
        // Pattern where central tile with 16 mask is transformed to 16
        AssertBlob47("""
                         ...
                         .#.
                         .#.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 18 mask is transformed to 16
        AssertBlob47("""
                         ..#
                         .#.
                         .#.
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 24 mask is transformed to 16
        AssertBlob47("""
                         ...
                         .#.
                         .##
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 26 mask is transformed to 16
        AssertBlob47("""
                         ..#
                         .#.
                         .##
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 48 mask is transformed to 16
        AssertBlob47("""
                         ...
                         .#.
                         ##.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 50 mask is transformed to 16
        AssertBlob47("""
                         ..#
                         .#.
                         ##.
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 56 mask is transformed to 16
        AssertBlob47("""
                         ...
                         .#.
                         ###
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   69,   64, }, 
                         });
        
        // Pattern where central tile with 58 mask is transformed to 16
        AssertBlob47("""
                         ..#
                         .#.
                         ###
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   69,   64, }, 
                         });
        
        // Pattern where central tile with 144 mask is transformed to 16
        AssertBlob47("""
                         #..
                         .#.
                         .#.
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 146 mask is transformed to 16
        AssertBlob47("""
                         #.#
                         .#.
                         .#.
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 152 mask is transformed to 16
        AssertBlob47("""
                         #..
                         .#.
                         .##
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 154 mask is transformed to 16
        AssertBlob47("""
                         #.#
                         .#.
                         .##
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 176 mask is transformed to 16
        AssertBlob47("""
                         #..
                         .#.
                         ##.
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 178 mask is transformed to 16
        AssertBlob47("""
                         #.#
                         .#.
                         ##.
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 184 mask is transformed to 16
        AssertBlob47("""
                         #..
                         .#.
                         ###
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   69,   64, }, 
                         });
        
        // Pattern where central tile with 186 mask is transformed to 16
        AssertBlob47("""
                         #.#
                         .#.
                         ###
                         """, new[,] {
                         {   0,   -1,    0, }, 
                         {  -1,   16,   -1, }, 
                         {   4,   69,   64, }, 
                         });
   }
    // | 17|   | 19| 25| 27| 49| 51| 57| 59|145|147|153|155|177|179|185|187|
    // | # |   | ##| # | ##| # | ##| # | ##|## |###|## |###|## |###|## |###|
    // | # |   | # | # | # | # | # | # | # | # | # | # | # | # | # | # | # |
    // | # |   | # | ##| ##|## |## |###|###| # | # | ##| ##|## |## |###|###|
    [Test(Description="17 when 17,19,25,27,49,51,57,59,145,147,153,155,177,179,185,187")]
    public void TestTile17() {
        
        // Pattern where central tile with 17 mask is transformed to 17
        AssertBlob47("""
                         .#.
                         .#.
                         .#.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 19 mask is transformed to 17
        AssertBlob47("""
                         .##
                         .#.
                         .#.
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 25 mask is transformed to 17
        AssertBlob47("""
                         .#.
                         .#.
                         .##
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 27 mask is transformed to 17
        AssertBlob47("""
                         .##
                         .#.
                         .##
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 49 mask is transformed to 17
        AssertBlob47("""
                         .#.
                         .#.
                         ##.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 51 mask is transformed to 17
        AssertBlob47("""
                         .##
                         .#.
                         ##.
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 57 mask is transformed to 17
        AssertBlob47("""
                         .#.
                         .#.
                         ###
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   69,   64, }, 
                         });
        
        // Pattern where central tile with 59 mask is transformed to 17
        AssertBlob47("""
                         .##
                         .#.
                         ###
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   69,   64, }, 
                         });
        
        // Pattern where central tile with 145 mask is transformed to 17
        AssertBlob47("""
                         ##.
                         .#.
                         .#.
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 147 mask is transformed to 17
        AssertBlob47("""
                         ###
                         .#.
                         .#.
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 153 mask is transformed to 17
        AssertBlob47("""
                         ##.
                         .#.
                         .##
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 155 mask is transformed to 17
        AssertBlob47("""
                         ###
                         .#.
                         .##
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,   17,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 177 mask is transformed to 17
        AssertBlob47("""
                         ##.
                         .#.
                         ##.
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 179 mask is transformed to 17
        AssertBlob47("""
                         ###
                         .#.
                         ##.
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 185 mask is transformed to 17
        AssertBlob47("""
                         ##.
                         .#.
                         ###
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   69,   64, }, 
                         });
        
        // Pattern where central tile with 187 mask is transformed to 17
        AssertBlob47("""
                         ###
                         .#.
                         ###
                         """, new[,] {
                         {   4,   84,   64, }, 
                         {  -1,   17,   -1, }, 
                         {   4,   69,   64, }, 
                         });
   }
    // | 20|   | 22| 52| 54|148|150|180|182|
    // |   |   |  #|   |  #|#  |# #|#  |# #|
    // | ##|   | ##| ##| ##| ##| ##| ##| ##|
    // | # |   | # |## |## | # | # |## |## |
    [Test(Description="20 when 20,22,52,54,148,150,180,182")]
    public void TestTile20() {
        
        // Pattern where central tile with 20 mask is transformed to 20
        AssertBlob47("""
                         ...
                         .##
                         .#.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   20,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 22 mask is transformed to 20
        AssertBlob47("""
                         ..#
                         .##
                         .#.
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,   20,   65, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 52 mask is transformed to 20
        AssertBlob47("""
                         ...
                         .##
                         ##.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   20,   64, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 54 mask is transformed to 20
        AssertBlob47("""
                         ..#
                         .##
                         ##.
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,   20,   65, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 148 mask is transformed to 20
        AssertBlob47("""
                         #..
                         .##
                         .#.
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   20,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 150 mask is transformed to 20
        AssertBlob47("""
                         #.#
                         .##
                         .#.
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,   20,   65, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 180 mask is transformed to 20
        AssertBlob47("""
                         #..
                         .##
                         ##.
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   20,   64, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 182 mask is transformed to 20
        AssertBlob47("""
                         #.#
                         .##
                         ##.
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,   20,   65, }, 
                         {   4,   65,   -1, }, 
                         });
   }
    // | 21|   | 53|149|181|
    // | # |   | # |## |## |
    // | ##|   | ##| ##| ##|
    // | # |   |## | # |## |
    [Test(Description="21 when 21,53,149,181")]
    public void TestTile21() {
        
        // Pattern where central tile with 21 mask is transformed to 21
        AssertBlob47("""
                         .#.
                         .##
                         .#.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   21,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 53 mask is transformed to 21
        AssertBlob47("""
                         .#.
                         .##
                         ##.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   21,   64, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 149 mask is transformed to 21
        AssertBlob47("""
                         ##.
                         .##
                         .#.
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   21,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 181 mask is transformed to 21
        AssertBlob47("""
                         ##.
                         .##
                         ##.
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   21,   64, }, 
                         {   4,   65,   -1, }, 
                         });
   }
    // | 23|   | 55|151|183|
    // | ##|   | ##|###|###|
    // | ##|   | ##| ##| ##|
    // | # |   |## | # |## |
    [Test(Description="23 when 23,55,151,183")]
    public void TestTile23() {
        
        // Pattern where central tile with 23 mask is transformed to 23
        AssertBlob47("""
                         .##
                         .##
                         .#.
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,   23,  193, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 55 mask is transformed to 23
        AssertBlob47("""
                         .##
                         .##
                         ##.
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,   23,  193, }, 
                         {   4,   65,   -1, }, 
                         });
        
        // Pattern where central tile with 151 mask is transformed to 23
        AssertBlob47("""
                         ###
                         .##
                         .#.
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,   23,  193, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 183 mask is transformed to 23
        AssertBlob47("""
                         ###
                         .##
                         ##.
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,   23,  193, }, 
                         {   4,   65,   -1, }, 
                         });
   }
    // | 28|   | 30| 60| 62|156|158|188|190|
    // |   |   |  #|   |  #|#  |# #|#  |# #|
    // | ##|   | ##| ##| ##| ##| ##| ##| ##|
    // | ##|   | ##|###|###| ##| ##|###|###|
    [Test(Description="28 when 28,30,60,62,156,158,188,190")]
    public void TestTile28() {
        
        // Pattern where central tile with 28 mask is transformed to 28
        AssertBlob47("""
                         ...
                         .##
                         .##
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   28,  112, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 30 mask is transformed to 28
        AssertBlob47("""
                         ..#
                         .##
                         .##
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,   28,  113, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 60 mask is transformed to 28
        AssertBlob47("""
                         ...
                         .##
                         ###
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  -1,   28,  112, }, 
                         {   4,   71,  193, }, 
                         });
        
        // Pattern where central tile with 62 mask is transformed to 28
        AssertBlob47("""
                         ..#
                         .##
                         ###
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  -1,   28,  113, }, 
                         {   4,   71,  193, }, 
                         });
        
        // Pattern where central tile with 156 mask is transformed to 28
        AssertBlob47("""
                         #..
                         .##
                         .##
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   28,  112, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 158 mask is transformed to 28
        AssertBlob47("""
                         #.#
                         .##
                         .##
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,   28,  113, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 188 mask is transformed to 28
        AssertBlob47("""
                         #..
                         .##
                         ###
                         """, new[,] {
                         {   0,   -1,   -1, }, 
                         {  -1,   28,  112, }, 
                         {   4,   71,  193, }, 
                         });
        
        // Pattern where central tile with 190 mask is transformed to 28
        AssertBlob47("""
                         #.#
                         .##
                         ###
                         """, new[,] {
                         {   0,   -1,   16, }, 
                         {  -1,   28,  113, }, 
                         {   4,   71,  193, }, 
                         });
   }
    // | 29|   | 61|157|189|
    // | # |   | # |## |## |
    // | ##|   | ##| ##| ##|
    // | ##|   |###| ##|###|
    [Test(Description="29 when 29,61,157,189")]
    public void TestTile29() {
        
        // Pattern where central tile with 29 mask is transformed to 29
        AssertBlob47("""
                         .#.
                         .##
                         .##
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   29,  112, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 61 mask is transformed to 29
        AssertBlob47("""
                         .#.
                         .##
                         ###
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  -1,   29,  112, }, 
                         {   4,   71,  193, }, 
                         });
        
        // Pattern where central tile with 157 mask is transformed to 29
        AssertBlob47("""
                         ##.
                         .##
                         .##
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   29,  112, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 189 mask is transformed to 29
        AssertBlob47("""
                         ##.
                         .##
                         ###
                         """, new[,] {
                         {   4,   80,   -1, }, 
                         {  -1,   29,  112, }, 
                         {   4,   71,  193, }, 
                         });
   }
    // | 31|   | 63|159|191|
    // | ##|   | ##|###|###|
    // | ##|   | ##| ##| ##|
    // | ##|   |###| ##|###|
    [Test(Description="31 when 31,63,159,191")]
    public void TestTile31() {
        
        // Pattern where central tile with 31 mask is transformed to 31
        AssertBlob47("""
                         .##
                         .##
                         .##
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,   31,  241, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 63 mask is transformed to 31
        AssertBlob47("""
                         .##
                         .##
                         ###
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  -1,   31,  241, }, 
                         {   4,   71,  193, }, 
                         });
        
        // Pattern where central tile with 159 mask is transformed to 31
        AssertBlob47("""
                         ###
                         .##
                         .##
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,   31,  241, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 191 mask is transformed to 31
        AssertBlob47("""
                         ###
                         .##
                         ###
                         """, new[,] {
                         {   4,   92,  112, }, 
                         {  -1,   31,  241, }, 
                         {   4,   71,  193, }, 
                         });
   }
    // | 64|   | 66| 72| 74| 96| 98|104|106|192|194|200|202|224|226|232|234|
    // |   |   |  #|   |  #|   |  #|   |  #|#  |# #|#  |# #|#  |# #|#  |# #|
    // |## |   |## |## |## |## |## |## |## |## |## |## |## |## |## |## |## |
    // |   |   |   |  #|  #|#  |#  |# #|# #|   |   |  #|  #|#  |#  |# #|# #|
    [Test(Description="64 when 64,66,72,74,96,98,104,106,192,194,200,202,224,226,232,234")]
    public void TestTile64() {
        
        // Pattern where central tile with 64 mask is transformed to 64
        AssertBlob47("""
                         ...
                         ##.
                         ...
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   64,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 66 mask is transformed to 64
        AssertBlob47("""
                         ..#
                         ##.
                         ...
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {   4,   64,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 72 mask is transformed to 64
        AssertBlob47("""
                         ...
                         ##.
                         ..#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   64,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 74 mask is transformed to 64
        AssertBlob47("""
                         ..#
                         ##.
                         ..#
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {   4,   64,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 96 mask is transformed to 64
        AssertBlob47("""
                         ...
                         ##.
                         #..
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  20,   64,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 98 mask is transformed to 64
        AssertBlob47("""
                         ..#
                         ##.
                         #..
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  20,   64,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 104 mask is transformed to 64
        AssertBlob47("""
                         ...
                         ##.
                         #.#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  20,   64,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 106 mask is transformed to 64
        AssertBlob47("""
                         ..#
                         ##.
                         #.#
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  20,   64,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 192 mask is transformed to 64
        AssertBlob47("""
                         #..
                         ##.
                         ...
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   64,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 194 mask is transformed to 64
        AssertBlob47("""
                         #.#
                         ##.
                         ...
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {   5,   64,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 200 mask is transformed to 64
        AssertBlob47("""
                         #..
                         ##.
                         ..#
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   64,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 202 mask is transformed to 64
        AssertBlob47("""
                         #.#
                         ##.
                         ..#
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {   5,   64,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 224 mask is transformed to 64
        AssertBlob47("""
                         #..
                         ##.
                         #..
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  21,   64,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 226 mask is transformed to 64
        AssertBlob47("""
                         #.#
                         ##.
                         #..
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {  21,   64,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 232 mask is transformed to 64
        AssertBlob47("""
                         #..
                         ##.
                         #.#
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  21,   64,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 234 mask is transformed to 64
        AssertBlob47("""
                         #.#
                         ##.
                         #.#
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {  21,   64,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
   }
    // | 65|   | 67| 73| 75| 97| 99|105|107|
    // | # |   | ##| # | ##| # | ##| # | ##|
    // |## |   |## |## |## |## |## |## |## |
    // |   |   |   |  #|  #|#  |#  |# #|# #|
    [Test(Description="65 when 65,67,73,75,97,99,105,107")]
    public void TestTile65() {
        
        // Pattern where central tile with 65 mask is transformed to 65
        AssertBlob47("""
                         .#.
                         ##.
                         ...
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   65,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 67 mask is transformed to 65
        AssertBlob47("""
                         .##
                         ##.
                         ...
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {   4,   65,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 73 mask is transformed to 65
        AssertBlob47("""
                         .#.
                         ##.
                         ..#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   65,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 75 mask is transformed to 65
        AssertBlob47("""
                         .##
                         ##.
                         ..#
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {   4,   65,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 97 mask is transformed to 65
        AssertBlob47("""
                         .#.
                         ##.
                         #..
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  20,   65,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 99 mask is transformed to 65
        AssertBlob47("""
                         .##
                         ##.
                         #..
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  20,   65,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 105 mask is transformed to 65
        AssertBlob47("""
                         .#.
                         ##.
                         #.#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  20,   65,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 107 mask is transformed to 65
        AssertBlob47("""
                         .##
                         ##.
                         #.#
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  20,   65,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
   }
    // | 68|   | 70| 76| 78|100|102|108|110|196|198|204|206|228|230|236|238|
    // |   |   |  #|   |  #|   |  #|   |  #|#  |# #|#  |# #|#  |# #|#  |# #|
    // |###|   |###|###|###|###|###|###|###|###|###|###|###|###|###|###|###|
    // |   |   |   |  #|  #|#  |#  |# #|# #|   |   |  #|  #|#  |#  |# #|# #|
    [Test(Description="68 when 68,70,76,78,100,102,108,110,196,198,204,206,228,230,236,238")]
    public void TestTile68() {
        
        // Pattern where central tile with 68 mask is transformed to 68
        AssertBlob47("""
                         ...
                         ###
                         ...
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   68,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 70 mask is transformed to 68
        AssertBlob47("""
                         ..#
                         ###
                         ...
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {   4,   68,   65, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 76 mask is transformed to 68
        AssertBlob47("""
                         ...
                         ###
                         ..#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   68,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 78 mask is transformed to 68
        AssertBlob47("""
                         ..#
                         ###
                         ..#
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {   4,   68,   81, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 100 mask is transformed to 68
        AssertBlob47("""
                         ...
                         ###
                         #..
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  20,   68,   64, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 102 mask is transformed to 68
        AssertBlob47("""
                         ..#
                         ###
                         #..
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  20,   68,   65, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 108 mask is transformed to 68
        AssertBlob47("""
                         ...
                         ###
                         #.#
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  20,   68,   80, }, 
                         {   1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 110 mask is transformed to 68
        AssertBlob47("""
                         ..#
                         ###
                         #.#
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  20,   68,   81, }, 
                         {   1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 196 mask is transformed to 68
        AssertBlob47("""
                         #..
                         ###
                         ...
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   68,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 198 mask is transformed to 68
        AssertBlob47("""
                         #.#
                         ###
                         ...
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {   5,   68,   65, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 204 mask is transformed to 68
        AssertBlob47("""
                         #..
                         ###
                         ..#
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   68,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 206 mask is transformed to 68
        AssertBlob47("""
                         #.#
                         ###
                         ..#
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {   5,   68,   81, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 228 mask is transformed to 68
        AssertBlob47("""
                         #..
                         ###
                         #..
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  21,   68,   64, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 230 mask is transformed to 68
        AssertBlob47("""
                         #.#
                         ###
                         #..
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {  21,   68,   65, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 236 mask is transformed to 68
        AssertBlob47("""
                         #..
                         ###
                         #.#
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  21,   68,   80, }, 
                         {   1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 238 mask is transformed to 68
        AssertBlob47("""
                         #.#
                         ###
                         #.#
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {  21,   68,   81, }, 
                         {   1,   -1,    1, }, 
                         });
   }
    // | 69|   | 77|101|109|
    // | # |   | # | # | # |
    // |###|   |###|###|###|
    // |   |   |  #|#  |# #|
    [Test(Description="69 when 69,77,101,109")]
    public void TestTile69() {
        
        // Pattern where central tile with 69 mask is transformed to 69
        AssertBlob47("""
                         .#.
                         ###
                         ...
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   69,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 77 mask is transformed to 69
        AssertBlob47("""
                         .#.
                         ###
                         ..#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   69,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 101 mask is transformed to 69
        AssertBlob47("""
                         .#.
                         ###
                         #..
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  20,   69,   64, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 109 mask is transformed to 69
        AssertBlob47("""
                         .#.
                         ###
                         #.#
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  20,   69,   80, }, 
                         {   1,   -1,    1, }, 
                         });
   }
    // | 71|   | 79|103|111|
    // | ##|   | ##| ##| ##|
    // |###|   |###|###|###|
    // |   |   |  #|#  |# #|
    [Test(Description="71 when 71,79,103,111")]
    public void TestTile71() {
        
        // Pattern where central tile with 71 mask is transformed to 71
        AssertBlob47("""
                         .##
                         ###
                         ...
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {   4,   71,  193, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 79 mask is transformed to 71
        AssertBlob47("""
                         .##
                         ###
                         ..#
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {   4,   71,  209, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 103 mask is transformed to 71
        AssertBlob47("""
                         .##
                         ###
                         #..
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  20,   71,  193, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 111 mask is transformed to 71
        AssertBlob47("""
                         .##
                         ###
                         #.#
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  20,   71,  209, }, 
                         {   1,   -1,    1, }, 
                         });
   }
    // | 80|   | 82| 88| 90|208|210|216|218|
    // |   |   |  #|   |  #|#  |# #|#  |# #|
    // |## |   |## |## |## |## |## |## |## |
    // | # |   | # | ##| ##| # | # | ##| ##|
    [Test(Description="80 when 80,82,88,90,208,210,216,218")]
    public void TestTile80() {
        
        // Pattern where central tile with 80 mask is transformed to 80
        AssertBlob47("""
                         ...
                         ##.
                         .#.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 82 mask is transformed to 80
        AssertBlob47("""
                         ..#
                         ##.
                         .#.
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {   4,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 88 mask is transformed to 80
        AssertBlob47("""
                         ...
                         ##.
                         .##
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   80,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 90 mask is transformed to 80
        AssertBlob47("""
                         ..#
                         ##.
                         .##
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {   4,   80,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 208 mask is transformed to 80
        AssertBlob47("""
                         #..
                         ##.
                         .#.
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 210 mask is transformed to 80
        AssertBlob47("""
                         #.#
                         ##.
                         .#.
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {   5,   80,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 216 mask is transformed to 80
        AssertBlob47("""
                         #..
                         ##.
                         .##
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   80,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 218 mask is transformed to 80
        AssertBlob47("""
                         #.#
                         ##.
                         .##
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {   5,   80,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
   }
    // | 81|   | 83| 89| 91|
    // | # |   | ##| # | ##|
    // |## |   |## |## |## |
    // | # |   | # | ##| ##|
    [Test(Description="81 when 81,83,89,91")]
    public void TestTile81() {
        
        // Pattern where central tile with 81 mask is transformed to 81
        AssertBlob47("""
                         .#.
                         ##.
                         .#.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   81,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 83 mask is transformed to 81
        AssertBlob47("""
                         .##
                         ##.
                         .#.
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {   4,   81,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 89 mask is transformed to 81
        AssertBlob47("""
                         .#.
                         ##.
                         .##
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   81,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 91 mask is transformed to 81
        AssertBlob47("""
                         .##
                         ##.
                         .##
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {   4,   81,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
   }
    // | 84|   | 86|212|214|
    // |   |   |  #|#  |# #|
    // |###|   |###|###|###|
    // | # |   | # | # | # |
    [Test(Description="84 when 84,86,212,214")]
    public void TestTile84() {
        
        // Pattern where central tile with 84 mask is transformed to 84
        AssertBlob47("""
                         ...
                         ###
                         .#.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   84,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 86 mask is transformed to 84
        AssertBlob47("""
                         ..#
                         ###
                         .#.
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {   4,   84,   65, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 212 mask is transformed to 84
        AssertBlob47("""
                         #..
                         ###
                         .#.
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   84,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 214 mask is transformed to 84
        AssertBlob47("""
                         #.#
                         ###
                         .#.
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {   5,   84,   65, }, 
                         {  -1,    1,   -1, }, 
                         });
   }
    // | 85|
    // | # |
    // |###|
    // | # |
    [Test(Description="85 when 85")]
    public void TestTile85() {
        
        // Pattern where central tile with 85 mask is transformed to 85
        AssertBlob47("""
                         .#.
                         ###
                         .#.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   85,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
   }
    // | 87|
    // | ##|
    // |###|
    // | # |
    [Test(Description="87 when 87")]
    public void TestTile87() {
        
        // Pattern where central tile with 87 mask is transformed to 87
        AssertBlob47("""
                         .##
                         ###
                         .#.
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {   4,   87,  193, }, 
                         {  -1,    1,   -1, }, 
                         });
   }
    // | 92|   | 94|220|222|
    // |   |   |  #|#  |# #|
    // |###|   |###|###|###|
    // | ##|   | ##| ##| ##|
    [Test(Description="92 when 92,94,220,222")]
    public void TestTile92() {
        
        // Pattern where central tile with 92 mask is transformed to 92
        AssertBlob47("""
                         ...
                         ###
                         .##
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {   4,   92,  112, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 94 mask is transformed to 92
        AssertBlob47("""
                         ..#
                         ###
                         .##
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {   4,   92,  113, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 220 mask is transformed to 92
        AssertBlob47("""
                         #..
                         ###
                         .##
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {   5,   92,  112, }, 
                         {  -1,    7,  193, }, 
                         });
        
        // Pattern where central tile with 222 mask is transformed to 92
        AssertBlob47("""
                         #.#
                         ###
                         .##
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {   5,   92,  113, }, 
                         {  -1,    7,  193, }, 
                         });
   }
    // | 93|
    // | # |
    // |###|
    // | ##|
    [Test(Description="93 when 93")]
    public void TestTile93() {
        
        // Pattern where central tile with 93 mask is transformed to 93
        AssertBlob47("""
                         .#.
                         ###
                         .##
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {   4,   93,  112, }, 
                         {  -1,    7,  193, }, 
                         });
   }
    // | 95|
    // | ##|
    // |###|
    // | ##|
    [Test(Description="95 when 95")]
    public void TestTile95() {
        
        // Pattern where central tile with 95 mask is transformed to 95
        AssertBlob47("""
                         .##
                         ###
                         .##
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {   4,   95,  241, }, 
                         {  -1,    7,  193, }, 
                         });
   }
    // |112|   |114|120|122|240|242|248|250|
    // |   |   |  #|   |  #|#  |# #|#  |# #|
    // |## |   |## |## |## |## |## |## |## |
    // |## |   |## |###|###|## |## |###|###|
    [Test(Description="112 when 112,114,120,122,240,242,248,250")]
    public void TestTile112() {
        
        // Pattern where central tile with 112 mask is transformed to 112
        AssertBlob47("""
                         ...
                         ##.
                         ##.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  28,  112,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 114 mask is transformed to 112
        AssertBlob47("""
                         ..#
                         ##.
                         ##.
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  28,  112,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 120 mask is transformed to 112
        AssertBlob47("""
                         ...
                         ##.
                         ###
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  28,  112,   -1, }, 
                         {   7,  197,   64, }, 
                         });
        
        // Pattern where central tile with 122 mask is transformed to 112
        AssertBlob47("""
                         ..#
                         ##.
                         ###
                         """, new[,] {
                         {  -1,   -1,    0, }, 
                         {  28,  112,   -1, }, 
                         {   7,  197,   64, }, 
                         });
        
        // Pattern where central tile with 240 mask is transformed to 112
        AssertBlob47("""
                         #..
                         ##.
                         ##.
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  29,  112,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 242 mask is transformed to 112
        AssertBlob47("""
                         #.#
                         ##.
                         ##.
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {  29,  112,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 248 mask is transformed to 112
        AssertBlob47("""
                         #..
                         ##.
                         ###
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  29,  112,   -1, }, 
                         {   7,  197,   64, }, 
                         });
        
        // Pattern where central tile with 250 mask is transformed to 112
        AssertBlob47("""
                         #.#
                         ##.
                         ###
                         """, new[,] {
                         {  16,   -1,    0, }, 
                         {  29,  112,   -1, }, 
                         {   7,  197,   64, }, 
                         });
   }
    // |113|   |115|121|123|
    // | # |   | ##| # | ##|
    // |## |   |## |## |## |
    // |## |   |## |###|###|
    [Test(Description="113 when 113,115,121,123")]
    public void TestTile113() {
        
        // Pattern where central tile with 113 mask is transformed to 113
        AssertBlob47("""
                         .#.
                         ##.
                         ##.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  28,  113,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 115 mask is transformed to 113
        AssertBlob47("""
                         .##
                         ##.
                         ##.
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  28,  113,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 121 mask is transformed to 113
        AssertBlob47("""
                         .#.
                         ##.
                         ###
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  28,  113,   -1, }, 
                         {   7,  197,   64, }, 
                         });
        
        // Pattern where central tile with 123 mask is transformed to 113
        AssertBlob47("""
                         .##
                         ##.
                         ###
                         """, new[,] {
                         {  -1,   20,   64, }, 
                         {  28,  113,   -1, }, 
                         {   7,  197,   64, }, 
                         });
   }
    // |116|   |118|244|246|
    // |   |   |  #|#  |# #|
    // |###|   |###|###|###|
    // |## |   |## |## |## |
    [Test(Description="116 when 116,118,244,246")]
    public void TestTile116() {
        
        // Pattern where central tile with 116 mask is transformed to 116
        AssertBlob47("""
                         ...
                         ###
                         ##.
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  28,  116,   64, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 118 mask is transformed to 116
        AssertBlob47("""
                         ..#
                         ###
                         ##.
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  28,  116,   65, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 244 mask is transformed to 116
        AssertBlob47("""
                         #..
                         ###
                         ##.
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  29,  116,   64, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 246 mask is transformed to 116
        AssertBlob47("""
                         #.#
                         ###
                         ##.
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {  29,  116,   65, }, 
                         {   7,  193,   -1, }, 
                         });
   }
    // |117|
    // | # |
    // |###|
    // |## |
    [Test(Description="117 when 117")]
    public void TestTile117() {
        
        // Pattern where central tile with 117 mask is transformed to 117
        AssertBlob47("""
                         .#.
                         ###
                         ##.
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  28,  117,   64, }, 
                         {   7,  193,   -1, }, 
                         });
   }
    // |119|
    // | ##|
    // |###|
    // |## |
    [Test(Description="119 when 119")]
    public void TestTile119() {
        
        // Pattern where central tile with 119 mask is transformed to 119
        AssertBlob47("""
                         .##
                         ###
                         ##.
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  28,  119,  193, }, 
                         {   7,  193,   -1, }, 
                         });
   }
    // |124|   |126|252|254|
    // |   |   |  #|#  |# #|
    // |###|   |###|###|###|
    // |###|   |###|###|###|
    [Test(Description="124 when 124,126,252,254")]
    public void TestTile124() {
        
        // Pattern where central tile with 124 mask is transformed to 124
        AssertBlob47("""
                         ...
                         ###
                         ###
                         """, new[,] {
                         {  -1,   -1,   -1, }, 
                         {  28,  124,  112, }, 
                         {   7,  199,  193, }, 
                         });
        
        // Pattern where central tile with 126 mask is transformed to 124
        AssertBlob47("""
                         ..#
                         ###
                         ###
                         """, new[,] {
                         {  -1,   -1,   16, }, 
                         {  28,  124,  113, }, 
                         {   7,  199,  193, }, 
                         });
        
        // Pattern where central tile with 252 mask is transformed to 124
        AssertBlob47("""
                         #..
                         ###
                         ###
                         """, new[,] {
                         {  16,   -1,   -1, }, 
                         {  29,  124,  112, }, 
                         {   7,  199,  193, }, 
                         });
        
        // Pattern where central tile with 254 mask is transformed to 124
        AssertBlob47("""
                         #.#
                         ###
                         ###
                         """, new[,] {
                         {  16,   -1,   16, }, 
                         {  29,  124,  113, }, 
                         {   7,  199,  193, }, 
                         });
   }
    // |125|
    // | # |
    // |###|
    // |###|
    [Test(Description="125 when 125")]
    public void TestTile125() {
        
        // Pattern where central tile with 125 mask is transformed to 125
        AssertBlob47("""
                         .#.
                         ###
                         ###
                         """, new[,] {
                         {  -1,   16,   -1, }, 
                         {  28,  125,  112, }, 
                         {   7,  199,  193, }, 
                         });
   }
    // |127|
    // | ##|
    // |###|
    // |###|
    [Test(Description="127 when 127")]
    public void TestTile127() {
        
        // Pattern where central tile with 127 mask is transformed to 127
        AssertBlob47("""
                         .##
                         ###
                         ###
                         """, new[,] {
                         {  -1,   28,  112, }, 
                         {  28,  127,  241, }, 
                         {   7,  199,  193, }, 
                         });
   }
    // |193|   |195|201|203|225|227|233|235|
    // |## |   |###|## |###|## |###|## |###|
    // |## |   |## |## |## |## |## |## |## |
    // |   |   |   |  #|  #|#  |#  |# #|# #|
    [Test(Description="193 when 193,195,201,203,225,227,233,235")]
    public void TestTile193() {
        
        // Pattern where central tile with 193 mask is transformed to 193
        AssertBlob47("""
                         ##.
                         ##.
                         ...
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  193,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 195 mask is transformed to 193
        AssertBlob47("""
                         ###
                         ##.
                         ...
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {   7,  193,   -1, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 201 mask is transformed to 193
        AssertBlob47("""
                         ##.
                         ##.
                         ..#
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  193,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 203 mask is transformed to 193
        AssertBlob47("""
                         ###
                         ##.
                         ..#
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {   7,  193,   -1, }, 
                         {  -1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 225 mask is transformed to 193
        AssertBlob47("""
                         ##.
                         ##.
                         #..
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  23,  193,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 227 mask is transformed to 193
        AssertBlob47("""
                         ###
                         ##.
                         #..
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {  23,  193,   -1, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 233 mask is transformed to 193
        AssertBlob47("""
                         ##.
                         ##.
                         #.#
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  23,  193,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
        
        // Pattern where central tile with 235 mask is transformed to 193
        AssertBlob47("""
                         ###
                         ##.
                         #.#
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {  23,  193,   -1, }, 
                         {   1,   -1,    0, }, 
                         });
   }
    // |197|   |205|229|237|
    // |## |   |## |## |## |
    // |###|   |###|###|###|
    // |   |   |  #|#  |# #|
    [Test(Description="197 when 197,205,229,237")]
    public void TestTile197() {
        
        // Pattern where central tile with 197 mask is transformed to 197
        AssertBlob47("""
                         ##.
                         ###
                         ...
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  197,   64, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 205 mask is transformed to 197
        AssertBlob47("""
                         ##.
                         ###
                         ..#
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  197,   80, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 229 mask is transformed to 197
        AssertBlob47("""
                         ##.
                         ###
                         #..
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  23,  197,   64, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 237 mask is transformed to 197
        AssertBlob47("""
                         ##.
                         ###
                         #.#
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  23,  197,   80, }, 
                         {   1,   -1,    1, }, 
                         });
   }
    // |199|   |207|231|239|
    // |###|   |###|###|###|
    // |###|   |###|###|###|
    // |   |   |  #|#  |# #|
    [Test(Description="199 when 199,207,231,239")]
    public void TestTile199() {
        
        // Pattern where central tile with 199 mask is transformed to 199
        AssertBlob47("""
                         ###
                         ###
                         ...
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {   7,  199,  193, }, 
                         {  -1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 207 mask is transformed to 199
        AssertBlob47("""
                         ###
                         ###
                         ..#
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {   7,  199,  209, }, 
                         {  -1,   -1,    1, }, 
                         });
        
        // Pattern where central tile with 231 mask is transformed to 199
        AssertBlob47("""
                         ###
                         ###
                         #..
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {  23,  199,  193, }, 
                         {   1,   -1,   -1, }, 
                         });
        
        // Pattern where central tile with 239 mask is transformed to 199
        AssertBlob47("""
                         ###
                         ###
                         #.#
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {  23,  199,  209, }, 
                         {   1,   -1,    1, }, 
                         });
   }
    // |209|   |211|217|219|
    // |## |   |###|## |###|
    // |## |   |## |## |## |
    // | # |   | # | ##| ##|
    [Test(Description="209 when 209,211,217,219")]
    public void TestTile209() {
        
        // Pattern where central tile with 209 mask is transformed to 209
        AssertBlob47("""
                         ##.
                         ##.
                         .#.
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  209,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 211 mask is transformed to 209
        AssertBlob47("""
                         ###
                         ##.
                         .#.
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {   7,  209,   -1, }, 
                         {  -1,    1,   -1, }, 
                         });
        
        // Pattern where central tile with 217 mask is transformed to 209
        AssertBlob47("""
                         ##.
                         ##.
                         .##
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  209,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
        
        // Pattern where central tile with 219 mask is transformed to 209
        AssertBlob47("""
                         ###
                         ##.
                         .##
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {   7,  209,   -1, }, 
                         {  -1,    5,   64, }, 
                         });
   }
    // |213|
    // |## |
    // |###|
    // | # |
    [Test(Description="213 when 213")]
    public void TestTile213() {
        
        // Pattern where central tile with 213 mask is transformed to 213
        AssertBlob47("""
                         ##.
                         ###
                         .#.
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  213,   64, }, 
                         {  -1,    1,   -1, }, 
                         });
   }
    // |215|
    // |###|
    // |###|
    // | # |
    [Test(Description="215 when 215")]
    public void TestTile215() {
        
        // Pattern where central tile with 215 mask is transformed to 215
        AssertBlob47("""
                         ###
                         ###
                         .#.
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {   7,  215,  193, }, 
                         {  -1,    1,   -1, }, 
                         });
   }
    // |221|
    // |## |
    // |###|
    // | ##|
    [Test(Description="221 when 221")]
    public void TestTile221() {
        
        // Pattern where central tile with 221 mask is transformed to 221
        AssertBlob47("""
                         ##.
                         ###
                         .##
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {   7,  221,  112, }, 
                         {  -1,    7,  193, }, 
                         });
   }
    // |223|
    // |###|
    // |###|
    // | ##|
    [Test(Description="223 when 223")]
    public void TestTile223() {
        
        // Pattern where central tile with 223 mask is transformed to 223
        AssertBlob47("""
                         ###
                         ###
                         .##
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {   7,  223,  241, }, 
                         {  -1,    7,  193, }, 
                         });
   }
    // |241|   |243|249|251|
    // |## |   |###|## |###|
    // |## |   |## |## |## |
    // |## |   |## |###|###|
    [Test(Description="241 when 241,243,249,251")]
    public void TestTile241() {
        
        // Pattern where central tile with 241 mask is transformed to 241
        AssertBlob47("""
                         ##.
                         ##.
                         ##.
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  31,  241,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 243 mask is transformed to 241
        AssertBlob47("""
                         ###
                         ##.
                         ##.
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {  31,  241,   -1, }, 
                         {   7,  193,   -1, }, 
                         });
        
        // Pattern where central tile with 249 mask is transformed to 241
        AssertBlob47("""
                         ##.
                         ##.
                         ###
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  31,  241,   -1, }, 
                         {   7,  197,   64, }, 
                         });
        
        // Pattern where central tile with 251 mask is transformed to 241
        AssertBlob47("""
                         ###
                         ##.
                         ###
                         """, new[,] {
                         {  28,  116,   64, }, 
                         {  31,  241,   -1, }, 
                         {   7,  197,   64, }, 
                         });
   }
    // |245|
    // |## |
    // |###|
    // |## |
    [Test(Description="245 when 245")]
    public void TestTile245() {
        
        // Pattern where central tile with 245 mask is transformed to 245
        AssertBlob47("""
                         ##.
                         ###
                         ##.
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  31,  245,   64, }, 
                         {   7,  193,   -1, }, 
                         });
   }
    // |247|
    // |###|
    // |###|
    // |## |
    [Test(Description="247 when 247")]
    public void TestTile247() {
        
        // Pattern where central tile with 247 mask is transformed to 247
        AssertBlob47("""
                         ###
                         ###
                         ##.
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {  31,  247,  193, }, 
                         {   7,  193,   -1, }, 
                         });
   }
    // |253|
    // |## |
    // |###|
    // |###|
    [Test(Description="253 when 253")]
    public void TestTile253() {
        
        // Pattern where central tile with 253 mask is transformed to 253
        AssertBlob47("""
                         ##.
                         ###
                         ###
                         """, new[,] {
                         {  28,  112,   -1, }, 
                         {  31,  253,  112, }, 
                         {   7,  199,  193, }, 
                         });
   }
    // |255|
    // |###|
    // |###|
    // |###|
    [Test(Description="255 when 255")]
    public void TestTile255() {
        
        // Pattern where central tile with 255 mask is transformed to 255
        AssertBlob47("""
                         ###
                         ###
                         ###
                         """, new[,] {
                         {  28,  124,  112, }, 
                         {  31,  255,  241, }, 
                         {   7,  199,  193, }, 
                         });
   }

}