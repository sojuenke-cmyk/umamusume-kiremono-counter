using UmaKiremonoCounter.Models;

namespace UmaKiremonoCounter.Services;

/// <summary>集計結果。</summary>
/// <param name="Occurrences">発生回数。</param>
/// <param name="Successes">成功回数。</param>
/// <param name="MissStreak">前回の成功から連続で切れ者を引けていない回数。</param>
public readonly record struct CategoryStats(int Occurrences, int Successes, int MissStreak)
{
    public double? SuccessRate => Occurrences == 0 ? null : (double)Successes / Occurrences;
}

public static class Statistics
{
    /// <summary>指定した種類の記録を古い順にたどって集計する。</summary>
    public static CategoryStats Compute(IEnumerable<CounterRecord> records, params EventKind[] kinds)
    {
        int occurrences = 0, successes = 0, missStreak = 0;
        foreach (var record in records)
        {
            if (!kinds.Contains(record.Kind))
            {
                continue;
            }

            occurrences++;
            if (record.Success)
            {
                successes++;
                missStreak = 0;
            }
            else
            {
                missStreak++;
            }
        }

        return new CategoryStats(occurrences, successes, missStreak);
    }
}
