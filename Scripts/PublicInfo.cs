using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicInfo : MonoBehaviour
{

    /*
     * 0: none
     * 1: dirt
     * 2: dirtTop
     * 3: sand
     * 4: oakLog
     * 5: oakLeaves
     * 6: cactus
     * 7: pork
     * 8: oakPlanks
     * 9: craftingTable
     */

    //general
    public readonly string[] blockNames = { "none", "dirt", "dirtTop", "sand", "oakLog", "oakLeaves", "cactus", "pork", "oakPlanks", "craftingTable" 
                                    };

    public int[] itemsPerCraft = { 0, 0, 0, 0, 0, 0, 0, 0, 4, 1 };

    //oakWood 
    public readonly int[,] oakPlanks_small_1 = { { 4, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 } }; // 1 oakLog = 4 oakPlankss
    public readonly int[,] oakPlanks_small_2 = { { 0, 0 }, { 4, 1 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] oakPlanks_small_3 = { { 0, 0 }, { 0, 0 }, { 4, 1 }, { 0, 0 } };
    public readonly int[,] oakPlanks_small_4 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 4, 1 } };

    public readonly int[,] oakPlanks_large_1 = { { 4, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } }; // 1 oakLog =  4 oakPlankss
    public readonly int[,] oakPlanks_large_2 = { { 0, 0 }, { 4, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] oakPlanks_large_3 = { { 0, 0 }, { 0, 0 }, { 4, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] oakPlanks_large_4 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 4, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] oakPlanks_large_5 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 4, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] oakPlanks_large_6 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 4, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] oakPlanks_large_7 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 4, 1 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] oakPlanks_large_8 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 4, 1 }, { 0, 0 } };
    public readonly int[,] oakPlanks_large_9 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 4, 1 } };

    //craftingTable 
    public readonly int[,] craftingTable_small = { { 8, 1 }, { 8, 1 }, { 8, 1 }, { 8, 1 } };

    public readonly int[,] craftingTable_large_1 = { { 8, 1 }, { 8, 1 }, { 0, 0 }, { 8, 1 }, { 8, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] craftingTable_large_2 = { { 0, 0 }, { 8, 1 }, { 8, 1 }, { 0, 0 }, { 8, 1 }, { 8, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public readonly int[,] craftingTable_large_3 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 8, 1 }, { 8, 1 }, { 0, 0 }, { 8, 1 }, { 8, 1 }, { 0, 0 } };
    public readonly int[,] craftingTable_large_4 = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 8, 1 }, { 8, 1 }, { 0, 0 }, { 8, 1 }, { 8, 1 } };

    public readonly List<int[,]> allRecipes_small = new List<int[,]>();
    public readonly List<int[,]> allRecipes_large = new List<int[,]>();

    public readonly int[] amountOfRecipesPerBlock_small = { 0, 0, 0, 0, 0, 0, 0, 0, 4, 1 };
    public readonly int[] amountOfRecipesPerBlock_large = { 0, 0, 0, 0, 0, 0, 0, 0, 9, 4 };

    public void Awake()
    {

        allRecipes_small.Add(oakPlanks_small_1);
        allRecipes_small.Add(oakPlanks_small_2);
        allRecipes_small.Add(oakPlanks_small_3);
        allRecipes_small.Add(oakPlanks_small_4);
        allRecipes_small.Add(craftingTable_small);

        allRecipes_large.Add(oakPlanks_large_1);
        allRecipes_large.Add(oakPlanks_large_2);
        allRecipes_large.Add(oakPlanks_large_3);
        allRecipes_large.Add(oakPlanks_large_4);
        allRecipes_large.Add(oakPlanks_large_5);
        allRecipes_large.Add(oakPlanks_large_6);
        allRecipes_large.Add(oakPlanks_large_7);
        allRecipes_large.Add(oakPlanks_large_8);
        allRecipes_large.Add(oakPlanks_large_9);
        allRecipes_large.Add(craftingTable_large_1);
        allRecipes_large.Add(craftingTable_large_2);
        allRecipes_large.Add(craftingTable_large_3);
        allRecipes_large.Add(craftingTable_large_4);

    }

}
