using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        if (start > end)
        {
            return 0;
        }

        var budgets = _budgetRepo.GetAll();
        decimal total = 0;
        
        foreach (var budget in budgets)
        {
            var budgetStart = DateTime.ParseExact(budget.YearMonth + "01", "yyyyMMdd", null);
            var daysInMonth = DateTime.DaysInMonth(budgetStart.Year, budgetStart.Month);
            var dailyAmount = (decimal)budget.Amount / daysInMonth;
            var budgetEnd = budgetStart.AddMonths(1).AddDays(-1);

            var effectiveStart = start > budgetStart ? start : budgetStart;
            var effectiveEnd = end < budgetEnd ? end : budgetEnd;

            if (effectiveStart <= effectiveEnd)
            {
                total += ((effectiveEnd - effectiveStart).Days + 1)  * dailyAmount;
            }
        }

        return total;
    }
}

public interface IBudgetRepo
{
    List<Budget> GetAll();
}

public class Budget
{
    public string YearMonth { get; set; }
    public int Amount { get; set; }
}

