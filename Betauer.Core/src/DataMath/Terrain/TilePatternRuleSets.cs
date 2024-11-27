using System;
using System.Collections.Generic;

namespace Betauer.Core.DataMath.Terrain;

public static class TilePatternRuleSets {
    private static readonly Dictionary<string, Func<int, bool>> TilePatternRules = new() {
        { "!", (v) => v != 0 },
        { "#", (v) => v == 0 },
        { "?", (_) => true },
    };

    public static readonly TilePatternSet<int, int> Blob47 =
        new TilePatternSet<int, int> { DefaultRules = TilePatternRules }
            .Add(0, """
                    ? ! ?
                    ! # !
                    ? ! ?
                    """)
            .Add(1, """
                    ? # ?
                    ! # !
                    ? ! ?
                    """)
            .Add(4, """
                    ? ! ?
                    ! # #
                    ? ! ?
                    """)
            .Add(5, """
                    ? # !
                    ! # #
                    ? ! ?
                    """)
            .Add(7, """
                    ? # #
                    ! # #
                    ? ! ?
                    """)
            .Add(16, """
                     ? ! ?
                     ! # !
                     ? # ?
                     """)
            .Add(17, """
                     ? # ?
                     ! # !
                     ? # ?
                     """)
            .Add(20, """
                     ? ! ?
                     ! # #
                     ? # !
                     """)
            .Add(21, """
                     ? # !
                     ! # #
                     ? # !
                     """)
            .Add(23, """
                     ? # #
                     ! # #
                     ? # !
                     """)
            .Add(28, """
                     ? ! ?
                     ! # #
                     ? # #
                     """)
            .Add(29, """
                     ? # !
                     ! # #
                     ? # #
                     """)
            .Add(31, """
                     ? # #
                     ! # #
                     ? # #
                     """)
            .Add(64, """
                     ? ! ?
                     # # !
                     ? ! ?
                     """)
            .Add(64, """
                     ? ! ?
                     # # !
                     ? ! ?
                     """)
            .Add(65, """
                     ! # ?
                     # # !
                     ? ! ?
                     """)
            .Add(68, """
                     ? ! ?
                     # # #
                     ? ! ?
                     """)
            .Add(69, """
                     ! # !
                     # # #
                     ? ! ?
                     """)
            .Add(71, """
                     ! # #
                     # # #
                     ? ! ?
                     """)
            .Add(80, """
                     ? ! ?
                     # # !
                     ! # ?
                     """)
            .Add(81, """
                     ! # ?
                     # # !
                     ! # ?
                     """)
            .Add(84, """
                     ? ! ?
                     # # #
                     ! # !
                     """)
            .Add(85, """
                     ! # !
                     # # #
                     ! # !
                     """)
            .Add(87, """
                     ! # #
                     # # #
                     ! # !
                     """)
            .Add(92, """
                     ? ! ?
                     # # #
                     ! # #
                     """)
            .Add(93, """
                     ! # !
                     # # #
                     ! # #
                     """)
            .Add(95, """
                     ! # #
                     # # #
                     ! # #
                     """)
            .Add(112, """
                      ? ! ?
                      # # !
                      # # ?
                      """)
            .Add(113, """
                      ! # ?
                      # # !
                      # # ?
                      """)
            .Add(116, """
                      ? ! ?
                      # # #
                      # # !
                      """)
            .Add(117, """
                      ! # !
                      # # #
                      # # !
                      """)
            .Add(119, """
                      ! # #
                      # # #
                      # # !
                      """)
            .Add(124, """
                      ? ! ?
                      # # #
                      # # #
                      """)
            .Add(125, """
                      ! # !
                      # # #
                      # # #
                      """)
            .Add(127, """
                      ! # #
                      # # #
                      # # #
                      """)
            .Add(193, """
                      # # ?
                      # # !
                      ? ! ?
                      """)
            .Add(197, """
                      # # !
                      # # #
                      ? ! ?
                      """)
            .Add(199, """
                      # # #
                      # # #
                      ? ! ?
                      """)
            .Add(209, """
                      # # ?
                      # # !
                      ! # ?
                      """)
            .Add(213, """
                      # # !
                      # # #
                      ! # !
                      """)
            .Add(215, """
                      # # #
                      # # #
                      ! # !
                      """)
            .Add(221, """
                      # # !
                      # # #
                      ! # #
                      """)
            .Add(223, """
                      # # #
                      # # #
                      ! # #
                      """)
            .Add(241, """
                      # # ?
                      # # !
                      # # ?
                      """)
            .Add(241, """
                      # # ?
                      # # !
                      # # ?
                      """)
            .Add(245, """
                      # # !
                      # # #
                      # # !
                      """)
            .Add(247, """
                      # # #
                      # # #
                      # # !
                      """)
            .Add(253, """
                      # # !
                      # # #
                      # # #
                      """)
            .Add(255, """
                      # # #
                      # # #
                      # # #
                      """);
}