using Betauer.DI.Attributes;

namespace Betauer.DI.Tests; 

[TestRunner.Test]
public class ScannerScopeTests {


    public class ScopedService {
        
    }
    
    [Configuration]
    public class Configuration {
        [Singleton(Scope = "T1")] public ScopedService Service1 => new ScopedService();
    } 
    
    
    [TestRunner.Test]
    public void BasicTest() {
        var c = new Container();
        // c.Build(config => {
            // config.Scan<>();
        // })


    }
    
}