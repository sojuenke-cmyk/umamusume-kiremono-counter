using UmaKiremonoCounter.Services;
using UmaKiremonoCounter.ViewModels;

namespace UmaKiremonoCounter.Tests;

public class CategoryViewModelTests
{
    private static CategoryViewModel Create(bool hasOutcome, bool showStreak)
        => new("タイトル", "短縮", hasOutcome, showStreak, _ => { }, () => true);

    [Fact]
    public void 成功失敗と未取得連続を表示する()
    {
        var category = Create(hasOutcome: true, showStreak: true);
        category.Update(new CategoryStats(Occurrences: 10, Successes: 3, MissStreak: 2));
        Assert.Equal("成功 3/10　未取得連続 2", category.Summary);
    }

    [Fact]
    public void 未取得連続なしの場合は成功回数だけ表示する()
    {
        var category = Create(hasOutcome: true, showStreak: false);
        category.Update(new CategoryStats(Occurrences: 4, Successes: 1, MissStreak: 3));
        Assert.Equal("成功 1/4", category.Summary);
    }

    [Fact]
    public void 発生のみの場合は発生回数を表示する()
    {
        var category = Create(hasOutcome: false, showStreak: false);
        category.Update(new CategoryStats(Occurrences: 2, Successes: 2, MissStreak: 0));
        Assert.Equal("発生 2", category.Summary);
    }
}
