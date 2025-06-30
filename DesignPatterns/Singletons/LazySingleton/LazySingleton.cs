namespace DesignPrinciples;

public class LazySingleton
{
    // 用于保存唯一实例的静态字段
    private static Singleton? _instance;

    // 私有构造函数，防止外部实例化
    private Singleton()
    {
        Console.WriteLine
            ("Singleton 实例已创建");
    }

    // 提供访问实例的全局入口
    public static Singleton GetInstance() 
    {
        // 如果实例不存在，则创建一个新的实例
        if (_instance == null)
        {
            _instance = new Singleton();
        }
        return _instance;
    }

    public void ShowMessage()
    {
        Console.WriteLine("Hello from Singleton!");
    }

    public void ShowMessage2()
    {
        Console.WriteLine("Hello from Singleton 2");
    }
}
class Program
{
    static void Main()
    {
        Console.WriteLine("--- 单例模式示例 ---");

        // 获取 Singleton 实例
        Singleton instance1 = Singleton.GetInstance();
        Singleton instance2 = Singleton.GetInstance();

        // 验证两个实例是否是同一个对象
        Console.WriteLine(ReferenceEquals(instance1, instance2)); // 输出 True

        // 调用实例方法
        instance1.ShowMessage();
        instance2.ShowMessage2();
    }
}