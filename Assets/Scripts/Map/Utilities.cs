using UnityEngine;

public class Utilities
{
    //洗牌算法
    //将传进来的参数：_dataArray重新洗牌，打乱顺序
    public static T[] ShuffleArray<T>(T[] dataArray, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < dataArray.Length; i++)
        {
            int randomNum = Random.Range(i, dataArray.Length);
            //Swap
            T temp = dataArray[randomNum];
            dataArray[randomNum] = dataArray[i];
            dataArray[i] = temp;
        }
        return dataArray;
    }
    /*
    假设：
        传进来test[]1，2，3，4，5，6
        for(int i = 0; i < 6; i++)
        i = 0
            int randomNum = Random.Range(0,6);[randomNum = 0~5其中一个随机数] -- 设为4
            temp = test[4] = 4;
            test[4] = test[0] = 1;
            test[0] = temp = 4
            .     .
        --> 4,2,3,1,5,6
        i = 1
            randomNum = Random.Range(1,6);[此时队列第一个已经排好] -- 设为3
            temp = test[3] = 3;
            test[3] = test[1] = 2;
            test[1] = temp = 3;
              .   .
        --> 4,1,3,2,5,6
    */
}
