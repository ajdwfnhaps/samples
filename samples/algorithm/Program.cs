// See https://aka.ms/new-console-template for more information

//回溯算法
//电话号码的字母组合

var res = new List<char[]>();
var s = new Stack<char>();


var letterMap = new string[] {
    "", // 0
    "", // 1
    "abc", // 2
    "def", // 3
    "ghi", // 4
    "jkl", // 5
    "mno", // 6
    "pqrs", // 7
    "tuv", // 8
    "wxyz", // 9
};

var str = Console.ReadLine();
backtracking(str, 0);

Console.WriteLine();
foreach (var r in res)
{
    Console.WriteLine($"{new string(r)}");
}

void backtracking(string digits, int index)
{
    /*
      if (终止条件) {
            存放结果;
            return;
        }

        for (选择：本层集合中元素（树中节点孩子的数量就是集合的大小）) {
            处理节点;
            backtracking(路径，选择列表); // 递归
            回溯，撤销处理结果
        }
     */

    if (index == digits.Length)
    {
        res.Add(s.Reverse().ToArray());
        return;
    }

    var cIndex = digits[index] - '0';
    var letters = letterMap[cIndex];

    foreach (var letter in letters)
    {
        s.Push(letter);
        backtracking(digits, index + 1);
        s.Pop();
    }

}