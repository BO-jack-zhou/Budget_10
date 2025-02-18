using NSubstitute;

namespace UnitTest;

[TestFixture]
public class BudgetServiceTests
{
    private IBudgetRepo? _budgetRepo;
    private BudgetService _budgetService;

    [SetUp]
    public void Setup()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);
    }
    
    [Test]
    public void no_budgets()
    {
        
        GivenBudget(new List<Budget>());

        var result = _budgetService.Query(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.AreEqual(0m, result);
    }

    private void GivenBudget(List<Budget> budgets)
    {
        _budgetRepo.GetAll().Returns(budgets);
    }

    [Test]
    public void query_partial_month()
    {
        GivenBudget(new List<Budget>
        {
            new Budget { YearMonth = "202401", Amount = 3100 } // $100 per day
        });
        
        var result = _budgetService.Query(new DateTime(2024, 1, 10), new DateTime(2024, 1, 20));

        Assert.AreEqual(1100m, result); // 11 days * $100
    }
    
    [Test]
    public void query_whole_month()
    {
        GivenBudget(new List<Budget>
        {
            new Budget { YearMonth = "202401", Amount = 3100 } // $100 per day
        });


        var result = _budgetService.Query(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.AreEqual(3100m, result);
    }
    
    [Test]
    public void query_one_day()
    {
        GivenBudget(new List<Budget>
        {
            new Budget { YearMonth = "202401", Amount = 3100 } // $100 per day
        });

        var result = _budgetService.Query(new DateTime(2024, 1, 1), new DateTime(2024, 1, 1));

        Assert.AreEqual(100m, result);
    }
    
    [Test]
    public void query_cross_two_month()
    {
        GivenBudget(new List<Budget>
        {
            new Budget { YearMonth = "202401", Amount = 6200 }, // $200 per day
            new Budget { YearMonth = "202402", Amount = 2900 } // $100 per day
        });

        var result = _budgetService.Query(new DateTime(2024, 1, 31), new DateTime(2024, 2, 1));

        Assert.AreEqual(300m, result); // 1 day in Jan, 1 day in Feb
    }
    
    
    
    [Test]
    public void end_date_earlier_than_start_date()
    {
        GivenBudget(new List<Budget>
        {
            new Budget { YearMonth = "202401", Amount = 3100 } // $100 per day
        });

        var result = _budgetService.Query(new DateTime(2024, 1, 31), new DateTime(2024, 1, 1));

        Assert.AreEqual(0m, result);
    }
    
}
