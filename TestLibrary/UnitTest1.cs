namespace TestLibrary;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        int [,] a = new int[,] { { 0, 11, 5}, 
                                  {11, 0, 6},
                                  {5, 6, 0}};
        int n = 3;
        Route o = new Route(n, a);
        o.Order.Length.Should().Be(n);
        int [] flags = {0, 0 , 0};
        for (int i = 0; i < n; ++i){
            flags[o.Order[i]] += 1;
        }
        for (int i = 0; i < n; ++i){
            flags[i].Should().Be(1);
        }
        int[] order = {0, 1, 2};
        o = new Route(order, a);
        o.TotalDistance.Should().Be(22);

        o = o.ChangeOrder();
        int j = 0;
        for (int i = 0; i < n; ++i){
            if (o.Order[i] == order[i]){
                j++;
            }
        }
        j.Should().Be(n - 2);
    }

    [Fact]
    public void Test2()
    {
        int [,] a = new int[,] { { 0, 11, 5}, 
                                  {11, 0, 6},
                                  {5, 6, 0}};
        int size = 10;
        Generation p = new Generation(size, a, 0.5);
        p.Individuals.Length.Should().Be(size);
        p.Step();
        p.BestDistance.Should().Be(22);
    }
}