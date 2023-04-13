using System.Collections.Generic;

namespace Betauer.TestRunner;

public class TestReport {
    public int TestsTotal { get; internal set; } = 0;
    public int TestsExecuted { get; internal set; } = 0;
    public int TestsFailed { get; internal set; } = 0;
    public int TestsPassed { get; internal set; } = 0;
    public List<TestRunner.TestMethod> TestsFailedResults { get;  } = new();
}