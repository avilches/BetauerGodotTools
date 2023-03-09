using System;
using System.Collections.Generic;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests; 

[TestFixture]
public class ScannerBasicTests : Node {
    [SetUp]
    public void Setup() {
        LoggerFactory.OverrideTraceLevel(TraceLevel.All);
    }

    public interface INotTagged {
    }

    [Service]
    public class MyServiceWithNotScanned {
        [Inject] internal INotTagged notFound { get; set; }
    }

    [Test(Description = "Types not found")]
    public void NotFound() {
        var di = new Container.Builder();
        di.Scan<INotTagged>();
        di.Scan<MyServiceWithNotScanned>();
        Assert.Throws<InjectMemberException>(() => di.Build());
    }

    [Service]
    [Configuration]
    public class WrongCombination1 {
    }

    [Factory]
    [Configuration]
    public class WrongCombination2 {
    }

    [Factory]
    [Service]
    public class WrongCombination3 {
    }

    [Test(Description = "Can't use [Configuration] [Fatory] and [Service] in the same class")]
    public void WrongCombinationTest() {
        Assert.Throws<InvalidAttributeException>(() => new Container.Builder().Scan<WrongCombination1>());
        Assert.Throws<InvalidAttributeException>(() => new Container.Builder().Scan<WrongCombination2>());
        Assert.Throws<InvalidAttributeException>(() => new Container.Builder().Scan<WrongCombination3>());
    }

    [Service]
    public class MyServiceWithWithNullable {
        [Inject(Nullable = true)] internal INotTagged nullable { get; set; }
    }

    [Test(Description = "Nullable")]
    public void Nullable() {
        var di = new Container.Builder();
        di.Scan<MyServiceWithWithNullable>();
        var c = di.Build();
        Assert.That(!c.Contains<INotTagged>());
        var x = c.Resolve<MyServiceWithWithNullable>();
        Assert.That(x.nullable, Is.Null);
    }

    [Service]
    public class InjectClass : IInjectable {
        public static Node n1;
        public static Node n2;
        public static Node n3;
        public static Node n4;
        public static Node n5;

        public void AssertInjectClass() {
            Assert.That(node1, Is.EqualTo(n1));
            Assert.That(node2, Is.EqualTo(n2));
            Assert.That(node3, Is.EqualTo(n3));
            Assert.That(_node4, Is.EqualTo(n4));
            Assert.That(_node5, Is.EqualTo(n5));
        }
            
        [Inject] public Node node3 { get; set; }
        [Inject] protected Node node2 { get; set; }
        [Inject] private Node node1 { get; set; }

        private Node _node4;
        [Inject]
        public void node4(Node n) {
            _node4 = n;

        }

        private Node _node5;
        [Inject("node5")]
        public void node5method(Node n) {
            _node5 = n;
        }

        internal int p1 = 0;
        public void PostInject() {
            p1++;
        }
    }

    [Test(Description = "Inject test: properties and method")]
    public void InjectTest() {
        InjectClass.n1 = new Node();
        InjectClass.n2 = new Node();
        InjectClass.n3 = new Node();
        InjectClass.n4 = new Node();
        InjectClass.n5 = new Node();
        var di = new Container.Builder();
        di.Register(Provider.Static(InjectClass.n1, "node1"));
        di.Register(Provider.Static(InjectClass.n2, "node2"));
        di.Register(Provider.Static(InjectClass.n3, "node3"));
        di.Register(Provider.Static(InjectClass.n4, "node4"));
        di.Register(Provider.Static(InjectClass.n5, "node5"));
            
        di.Scan<InjectClass>();
        var c = di.Build();

        var i = c.Resolve<InjectClass>();
        i.AssertInjectClass();
        Assert.That(i.p1, Is.EqualTo(1));

        c.InjectServices(i);
        i.AssertInjectClass();
        Assert.That(i.p1, Is.EqualTo(2));
    }

    [Test(Description = "Inject instance not created by Container")]
    public void ManualInjectTest() {
        InjectClass.n1 = new Node();
        InjectClass.n2 = new Node();
        InjectClass.n3 = new Node();
        InjectClass.n4 = new Node();
        InjectClass.n5 = new Node();
        var di = new Container.Builder();
        di.Register(Provider.Static(InjectClass.n1, "node1"));
        di.Register(Provider.Static(InjectClass.n2, "node2"));
        di.Register(Provider.Static(InjectClass.n3, "node3"));
        di.Register(Provider.Static(InjectClass.n4, "node4"));
        di.Register(Provider.Static(InjectClass.n5, "node5"));
        var c = di.Build();
            
        var i = new InjectClass();
        c.InjectServices(i);
            
        i.AssertInjectClass();
        Assert.That(i.p1, Is.EqualTo(1));

        c.InjectServices(i);
        i.AssertInjectClass();
        Assert.That(i.p1, Is.EqualTo(2));
    }

    [Service]
    public class ExposeServiceClass1 {
    }

    [Service(Name = "C")]
    public class ExposeServiceClass2 {
    }

    [Service(Type = typeof(INotTagged))]
    public class ExposeServiceClass3 : INotTagged {
    }

    [Test(Description = "Name or type")]
    public void NameOrTypeClass() {
        var di = new Container.Builder();
        di.Scan<ExposeServiceClass1>();
        di.Scan<ExposeServiceClass2>();
        di.Scan<ExposeServiceClass3>();
        var c = di.Build();
            
        // [Service] in class is registered by the class
        Assert.That(c.Contains<ExposeServiceClass1>());
        Assert.That(c.Resolve<ExposeServiceClass1>(), Is.TypeOf<ExposeServiceClass1>());
            
        // [Service(Name = "C"] in class is registered by the name and type
        Assert.That(c.Resolve("C"), Is.TypeOf<ExposeServiceClass2>());
        Assert.That(c.Resolve<ExposeServiceClass2>(), Is.TypeOf<ExposeServiceClass2>());

        // [Service(Type=typeof(INotTagged)] class is exposed by specified type
        Assert.That(!c.Contains<ExposeServiceClass3>()); 
        Assert.That(c.Resolve<INotTagged>(), Is.TypeOf<ExposeServiceClass3>());
    }

    [Service]
    [Primary]
    public class PrimaryTagClass {
    }
        
    [Test(Description = "Primary is false if there is no name specefied")]
    public void CheckPrimaryAttributeClasses() {
        var di = new Container.Builder();
        di.Scan<PrimaryTagClass>();
        var c = di.Build();
        Assert.That(c.GetProvider<PrimaryTagClass>().Primary, Is.False);
    }

    public class DummyClass {
    }

    [Configuration]
    public class PrimaryServiceConfiguration {
        [Service] private DummyClass noPrimary => new DummyClass();
        [Service] [Primary] private DummyClass primaryTag => new DummyClass();
        [Service(Primary = true)] private DummyClass primary => new DummyClass();
    }

    [Test(Description = "Check Primary attribute in configuration")]
    public void CheckPrimaryAttributeConfiguration() {
        var di = new Container.Builder();
        di.Scan<PrimaryServiceConfiguration>();
        var c = di.Build();
        Assert.That(c.GetProvider("noPrimary").Primary, Is.False);
        Assert.That(c.GetProvider("primaryTag").Primary, Is.True);
        Assert.That(c.GetProvider("primary").Primary, Is.True);

        Assert.That(c.Resolve<DummyClass>(), Is.EqualTo(c.Resolve("primary")));
    }

    public interface I1 { }
    public interface I2 { }

    public class ExposeServiceMember1 {}
    public class ExposeServiceMember2 {}
    public class ExposeServiceMember3 {}
    public class ExposeServiceMember4 : I1, I2 {}


    [Configuration]
    public class ConfigurationScanned {
        [Service] internal ExposeServiceMember1 member11 => new ExposeServiceMember1();
        [Service] internal ExposeServiceMember1 member12() => new ExposeServiceMember1();
        [Service(Name = "M21", Primary = true)] internal ExposeServiceMember2 member21 => new ExposeServiceMember2();
        [Service(Name = "M22")] internal ExposeServiceMember2 member22() => new ExposeServiceMember2();
            
        [Service(Name = "M3")] internal ExposeServiceMember3 member3 => new ExposeServiceMember3();
        [Service(Name = "M3P1", Primary = true)] internal ExposeServiceMember3 member3P1() => new ExposeServiceMember3();
        [Service(Name = "M3P2", Primary = true)] internal ExposeServiceMember3 member3P2() => new ExposeServiceMember3();
            
        [Service(Type = typeof(I1))] internal I1 member41 => new ExposeServiceMember4();
        [Service(Type = typeof(I2))] internal I2 member42() => new ExposeServiceMember4();
    }

    [Test(Description = "Name or type members in configuration instance")]
    public void NameOrTypeMembersInConfigurationInstance() {
        var di = new Container.Builder();
        di.ScanConfiguration(new ConfigurationScanned());
        var c = di.Build();
        AssertNameOrTypeMembers(c);
    }

    [Test(Description = "Name or type members in configuration class")]
    public void NameOrTypeMembersInConfigurationClass() {
        var di = new Container.Builder();
        di.Scan<ConfigurationScanned>();
        var c = di.Build();
        AssertNameOrTypeMembers(c);
    }

    private void AssertNameOrTypeMembers(Container c) {
        // [Service] member is exposed by variable or method name
        // Assert.That(c.Resolve<ExposeServiceMember1>(), Is.TypeOf<ExposeServiceMember1>()); 
        // Assert.That(c.Resolve("member11"), Is.TypeOf<ExposeServiceMember1>());
        // Assert.That(c.Resolve("member12"), Is.TypeOf<ExposeServiceMember1>());

        // [Service(Name="M")] member is exposed by name M and by type too (using the first one)
        Assert.That(c.Resolve("M21"), Is.TypeOf<ExposeServiceMember2>());
        Assert.That(c.Resolve("M22"), Is.TypeOf<ExposeServiceMember2>());
        Assert.That(c.Resolve<ExposeServiceMember2>(), Is.EqualTo(c.Resolve("M21"))); 
        Assert.That(!c.Contains("member21")); 
        Assert.That(!c.Contains("member22")); 

        // [Service(Name="M")] member is exposed by name M and by type too (using the first one)
        Assert.That(c.Resolve("M3"), Is.TypeOf<ExposeServiceMember3>());
        Assert.That(c.Resolve("M3P1"), Is.TypeOf<ExposeServiceMember3>());
        Assert.That(c.Resolve("M3P2"), Is.TypeOf<ExposeServiceMember3>());
        Assert.That(c.Resolve<ExposeServiceMember3>(), Is.EqualTo(c.Resolve("M3P2"))); 
        Assert.That(!c.Contains("member3")); 
        Assert.That(!c.Contains("member3P1")); 
        Assert.That(!c.Contains("member3P2")); 

        // [Service(Type=typeof(GodotObject)] member is exposed by specified type only, not the member type
        Assert.That(!c.Contains<ExposeServiceMember4>()); 
        Assert.That(c.Resolve<I1>(), Is.TypeOf<ExposeServiceMember4>());
        Assert.That(c.Resolve<I2>(), Is.TypeOf<ExposeServiceMember4>());
        Assert.That(c.Resolve<I1>("member41"), Is.TypeOf<ExposeServiceMember4>());
        Assert.That(c.Resolve<I2>("member42"), Is.TypeOf<ExposeServiceMember4>());
        Assert.That(c.Contains("member41")); 
        Assert.That(c.Contains("member42")); 
    }

    [Service(typeof(IInterface1), Name = "A")]
    public class ServiceByNameFallbackA : IInterface1 {
    }

    [Service(typeof(IInterface1), Name = "B")]
    public class ServiceByNameFallbackB : IInterface1 {
    }

    [Service(typeof(IInterface1), Name = "P1", Primary = true)]
    public class ServiceByNameFallbackP1 : IInterface1 {
    }

    [Service(typeof(IInterface1), Name = "P2", Primary = true)]
    public class ServiceByNameFallbackP2 : IInterface1 {
    }

    [Service(typeof(IInterface1), Name = "C")]
    public class ServiceByNameFallbackC : IInterface1 {
    }

    [Test(Description = "Check Primary works")]
    public void PrimaryTest() {
        var di = new Container.Builder();
        di.Scan<ServiceByNameFallbackA>();
        di.Scan<ServiceByNameFallbackB>();
        var c = di.Build();
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(c.Resolve<IInterface1>("A")));
        di.Scan<ServiceByNameFallbackP1>();
        di.Build();
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(c.Resolve<IInterface1>("P1")));
        di.Scan<ServiceByNameFallbackP2>();
        di.Scan<ServiceByNameFallbackC>();
        di.Build();
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(c.Resolve<IInterface1>("P2")));
    }


    [Service(Lifetime.Transient)]
    public class EmptyTransient {
        public static int Created = 0;

        public EmptyTransient() {
            Created++;
        }
    }

    [Service]
    public class SingletonWith2Transients {
        [Inject] internal EmptyTransient NotAllowed { get; set; }
    }

    [Test(Description = "Inject Transient in Singleton is not allowed")]
    public void TransientOnSingletonIsNotAllowed() {
        var di = new Container.Builder();
        di.Scan<EmptyTransient>();
        di.Scan<SingletonWith2Transients>();
        Assert.Throws<InjectMemberException>(() => di.Build());
    }

    [Service(Lifetime.Transient)]
    public class TransientService {
        public static int Created = 0;

        public TransientService() {
            Created++;
        }

        [Inject] internal EmptyTransient et1 { get; set; }
        [Inject] internal EmptyTransient et2 { get; set; }
    }

    [Test(Description = "Inject transients in transient")]
    public void TransientInTransient() {
        var di = new Container.Builder();
        EmptyTransient.Created = 0;
        TransientService.Created = 0;
        EmptyTransient.Created = 0;

        di.Scan<EmptyTransient>();
        di.Scan<TransientService>();
        var c = di.Build();
        
        var ts2 = c.Resolve<TransientService>();
        Assert.That(TransientService.Created, Is.EqualTo(1));
        Assert.That(EmptyTransient.Created, Is.EqualTo(2));
        Assert.That(ts2.et1, Is.TypeOf<EmptyTransient>());
        Assert.That(ts2.et2, Is.TypeOf<EmptyTransient>());
        Assert.That(ts2.et1, Is.Not.EqualTo(ts2.et2));
    }
        
    public interface IMultipleImpByName {}

    [Service(Name = "M1")] public class MultipleImpl1ByName : IMultipleImpByName {}
    [Service(Name = "M2")] public class MultipleImpl2ByName : IMultipleImpByName {}
    [Service(Name = "M3")] public class MultipleImpl3ByName : IMultipleImpByName {}

    [Service]
    public class ServiceWithMultipleImpl1 {
        [Inject(Name = "M1")] internal IMultipleImpByName mul11 { get; set; }
        [Inject(Name = "M1")] internal IMultipleImpByName mul12 { get; set; }
            
        [Inject(Name = "M2")] internal IMultipleImpByName mul21 { get; set; }
        [Inject(Name = "M2")] internal IMultipleImpByName mul22 { get; set; }
            
        [Inject(Name = "M3")] internal IMultipleImpByName mul31t { get; set; }
        [Inject(Name = "M3")] internal IMultipleImpByName mul32t { get; set; }
    }

    [Service]
    public class ServiceWithMultipleImpl2 {
        [Inject(Name = "M1")] internal IMultipleImpByName mul11 { get; set; }
        [Inject(Name = "M1")] internal IMultipleImpByName mul12 { get; set; }
            
        [Inject(Name = "M2")] internal IMultipleImpByName mul21 { get; set; }
        [Inject(Name = "M2")] internal IMultipleImpByName mul22 { get; set; }
            
        [Inject(Name = "M3")] internal IMultipleImpByName mul31t { get; set; }
        [Inject(Name = "M3")] internal IMultipleImpByName mul32t { get; set; }
    }
    [Test(Description = "When an interface has multiple implementations, register by name")]
    public void InterfaceWithMultipleImplementations() {
        var di = new Container.Builder();
        di.Scan<MultipleImpl1ByName>();
        di.Scan<MultipleImpl2ByName>();
        di.Scan<MultipleImpl3ByName>();
        di.Scan<ServiceWithMultipleImpl1>();
        di.Scan<ServiceWithMultipleImpl2>();
        var c = di.Build();
        var i1 = c.Resolve<IMultipleImpByName>("M1");
        var i2 = c.Resolve<IMultipleImpByName>("M2");
        var s1 = c.Resolve<ServiceWithMultipleImpl1>();
        var s2 = c.Resolve<ServiceWithMultipleImpl2>();

        Assert.That(s1.mul11, Is.EqualTo(i1));
        Assert.That(s1.mul12, Is.EqualTo(i1));
        Assert.That(s1.mul21, Is.EqualTo(i2));
        Assert.That(s1.mul22, Is.EqualTo(i2));

        Assert.That(s2.mul11, Is.EqualTo(i1));
        Assert.That(s2.mul12, Is.EqualTo(i1));
        Assert.That(s2.mul21, Is.EqualTo(i2));
        Assert.That(s2.mul22, Is.EqualTo(i2));
    }

    public interface IMultipleImpByType {}
        
    [Service(typeof(IMultipleImpByType))] public class MultipleImpl1ByType : IMultipleImpByType {}

    [Service]
    public class ServiceWithMultipleImplByType {
        [Inject] internal IMultipleImpByType service { get; set; }
    }

    [Test(Description = "When an interface has multiple implementations, register by one type")]
    public void InterfaceWithMultipleImplementationsByType() {
        var di = new Container.Builder();
        di.Scan<MultipleImpl1ByType>();
        di.Scan<ServiceWithMultipleImplByType>();
        var c = di.Build();
        var i1 = c.Resolve<IMultipleImpByType>();
        Assert.That(i1, Is.TypeOf<MultipleImpl1ByType>());

        var s1 = c.Resolve<ServiceWithMultipleImplByType>();
        Assert.That(s1.service, Is.EqualTo(i1));
    }


    class AutoTransient1 {
        [Inject] internal ExposeServiceClass1 Singleton { get; set; }
    }

    class AutoTransient2 {
        [Inject] internal AutoTransient1 auto { get; set; }
    }

    [Test(Description = "Create if not found")]
    public void CreateIfNotFound() {
        var di = new Container.Builder();
        di.Scan<ExposeServiceClass1>();
        var c = di.Build();
        c.CreateIfNotFound = true;

        var a1 = c.Resolve<AutoTransient1>();
        var a2 = c.Resolve<AutoTransient1>();

        Assert.That(a1, Is.TypeOf<AutoTransient1>());
        Assert.That(a1, Is.Not.EqualTo(a2));
        Assert.That(a1.Singleton, Is.EqualTo(c.Resolve<ExposeServiceClass1>()));

        var s1 = c.Resolve<AutoTransient2>();
        var s2 = c.Resolve<AutoTransient2>();

        Assert.That(s1, Is.Not.EqualTo(s2));
        Assert.That(s1.auto, Is.Not.EqualTo(s2.auto));
        Assert.That(s1.auto.Singleton, Is.EqualTo(c.Resolve<ExposeServiceClass1>()));
    }


    [Service]
    public class Hold {
        public string Name;

        public Hold(string name) {
            Name = name;
        }
    }

    [Configuration]
    public class SingletonHolder {

        // Property
        [Service]
        private Hold SingletonHold1 => new Hold("1");
            
        // Property with Name
        [Service(Name = "SingletonHold2")]
        private Hold _hold2 => new Hold("2");

        // Method
        [Service]
        public Hold SingletonHold3() {
            return new Hold("3");
        }

        // Method with name
        [Service(Name = "SingletonHold4")]
        public Hold Hold4() {
            return new Hold("4");
        }

    }

    [Service]
    public class SingletonInjected {
        [Inject] internal Hold SingletonHold1 { get; set; }
        [Inject(Name = "SingletonHold1")] internal Hold h1 { get; set; }
            
        [Inject] internal Hold SingletonHold2 { get; set; }
        [Inject(Name = "SingletonHold2")] internal Hold h2 { get; set; }
            
        [Inject] internal Hold SingletonHold3 { get; set; }
        [Inject(Name = "SingletonHold3")] internal Hold h3 { get; set; }
            
        [Inject] internal Hold SingletonHold4 { get; set; }
        [Inject(Name = "SingletonHold4")] internal Hold h4 { get; set; }
    }
        
    [Test(Description = "Inject singletons by name exported in a singleton")]
    public void ExportSingletonFrom() {
        var di = new Container.Builder();
        di.Scan<SingletonInjected>();
        di.Scan<SingletonHolder>();
        var c = di.Build();

        LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Info);
        LoggerFactory.SetTraceLevel(typeof(Container.Builder), TraceLevel.Info);
        SingletonInjected s1 = c.Resolve<SingletonInjected>();
        SingletonInjected s2 = c.Resolve<SingletonInjected>();
        Assert.That(s1, Is.EqualTo(s2));
            
        Assert.That(s1.h1.Name, Is.EqualTo("1"));
        Assert.That(s1.SingletonHold1.Name, Is.EqualTo("1"));
        Assert.That(s1.SingletonHold1, Is.EqualTo(s1.h1));
            
        Assert.That(s1.h2.Name, Is.EqualTo("2"));
        Assert.That(s1.SingletonHold2.Name, Is.EqualTo("2"));
        Assert.That(s1.SingletonHold2, Is.EqualTo(s1.h2));
            
        Assert.That(s1.h3.Name, Is.EqualTo("3"));
        Assert.That(s1.SingletonHold3.Name, Is.EqualTo("3"));
        Assert.That(s1.SingletonHold3, Is.EqualTo(s1.h3));
            
        Assert.That(s1.h4.Name, Is.EqualTo("4"));
        Assert.That(s1.SingletonHold4.Name, Is.EqualTo("4"));
        Assert.That(s1.SingletonHold4, Is.EqualTo(s1.h4));
    }
        
    [Configuration]
    public class TransientHolder {

        // Property
        [Service(Lifetime.Transient)]
        private Hold Hold1 => new Hold("1");
            
        // Property with Name
        [Service(Lifetime.Transient, Name = "Hold2")]
        private Hold _hold2 => new Hold("2");

        // Method
        [Service(Lifetime.Transient)]
        public Hold Hold3() {
            return new Hold("3");
        }

        // Method with name
        [Service(Lifetime.Transient, Name = "Hold4")]
        public Hold Hold4() {
            return new Hold("4");
        }

    }

    [Service(Lifetime.Transient)]
    public class TransientInjected {
        [Inject] internal Hold Hold1 { get; set; }
        [Inject(Name = "Hold1")] internal Hold h1 { get; set; }
            
        [Inject] internal Hold Hold2 { get; set; }
        [Inject(Name = "Hold2")] internal Hold h2 { get; set; }
            
        [Inject] internal Hold Hold3 { get; set; }
        [Inject(Name = "Hold3")] internal Hold h3 { get; set; }
            
        [Inject] internal Hold Hold4 { get; set; }
        [Inject(Name = "Hold4")] internal Hold h4 { get; set; }
    }
        
    [Test(Description = "Inject Transients by name exported in a Transient")]
    public void ExportTransientFrom() {
        var di = new Container.Builder();
        di.Scan<TransientInjected>();
        di.Scan<TransientHolder>();
        var c = di.Build();

        LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Info);
        LoggerFactory.SetTraceLevel(typeof(Container.Builder), TraceLevel.Info);
        TransientInjected s1 = c.Resolve<TransientInjected>();
        TransientInjected s2 = c.Resolve<TransientInjected>();
        Assert.That(s1, Is.Not.EqualTo(s2));
            
        Assert.That(s1.h1.Name, Is.EqualTo("1"));
        Assert.That(s1.Hold1.Name, Is.EqualTo("1"));
        Assert.That(s1.Hold1, Is.Not.EqualTo(s1.h1)); 
        Assert.That(s2.Hold1, Is.Not.EqualTo(s1.h1));
            
        Assert.That(s1.h2.Name, Is.EqualTo("2"));
        Assert.That(s1.Hold2.Name, Is.EqualTo("2"));
        Assert.That(s1.Hold2, Is.Not.EqualTo(s1.h2));
        Assert.That(s2.Hold2, Is.Not.EqualTo(s1.h2));
            
        Assert.That(s1.h3.Name, Is.EqualTo("3"));
        Assert.That(s1.Hold3.Name, Is.EqualTo("3"));
        Assert.That(s1.Hold3, Is.Not.EqualTo(s1.h3));
        Assert.That(s2.Hold3, Is.Not.EqualTo(s1.h3));
            
        Assert.That(s1.h4.Name, Is.EqualTo("4"));
        Assert.That(s1.Hold4.Name, Is.EqualTo("4"));
        Assert.That(s1.Hold4, Is.Not.EqualTo(s1.h4));
        Assert.That(s2.Hold4, Is.Not.EqualTo(s1.h4));
    }

    [Configuration]
    public class ConfigurationService {
        public static int Created = 0;
        public ConfigurationService() {
            Created++;
        }

        [Service(Lifetime.Transient)] private Hold TransientHold1 => new Hold("1");
        [Service] private Hold SingletonHold1() => new Hold("2");
    }

    [Test(Description = "Use configuration to export members")]
    public void ExportFromConfiguration() {
        var di = new Container.Builder();
        di.Scan<ConfigurationService>();
        var c = di.Build();

        Assert.That(ConfigurationService.Created, Is.EqualTo(1));

        Assert.That(c.Contains<ConfigurationService>(), Is.False);

        Hold t1 = c.Resolve<Hold>("TransientHold1");
        Hold t2 = c.Resolve<Hold>("TransientHold1");

        Assert.That(t1.Name, Is.EqualTo("1"));
        Assert.That(t2.Name, Is.EqualTo("1"));
        // Transients are all different
        Assert.That(t1, Is.Not.EqualTo(t2));

        // Singleton are the same
        Hold s11 = c.Resolve<Hold>("SingletonHold1");
        Hold s12 = c.Resolve<Hold>("SingletonHold1");
        Assert.That(s11, Is.EqualTo(s12));
    }

    [Scan<ServiceMemberExposing1>()]
    [Scan<ImportedService>()]
    internal class AddToScanByImport {
    }

    [Configuration]
    internal class ServiceMemberExposing1 {
        [Service] private ServiceMemberExposing2 member => new ServiceMemberExposing2();
    }
        
    internal class ServiceMemberExposing2 {
    }

    [Service]
    internal class ImportedService {
    }

    [Test]
    public void AddToScanByImportTest() {
        var di = new Container.Builder();
        di.Scan<AddToScanByImport>();
        var c = di.Build();
        Assert.That(c.Resolve<ImportedService>(), Is.TypeOf<ImportedService>());
        Assert.That(c.Resolve<ServiceMemberExposing2>(), Is.TypeOf<ServiceMemberExposing2>());
    }

    [Test]
    public void AddToScanByImportConfigurationInstanceTest() {
        var di = new Container.Builder();
        di.ScanConfiguration(new AddToScanByImport());
        var c = di.Build();
        Assert.That(c.Resolve<ImportedService>(), Is.TypeOf<ImportedService>());
        Assert.That(c.Resolve<ServiceMemberExposing2>(), Is.TypeOf<ServiceMemberExposing2>());
    }

        
    [Service(Lifetime.Transient)]
    public class PostInjectTransient : IInjectable {
        public static int Created = 0;

        public PostInjectTransient() {
            Created++;
        }
        public int Called = 0;
        public void PostInject() {
            Called++;
        }
    }

    [Service(Lifetime.Transient)]
    class TransientWithTransient {
        [Inject] public PostInjectTransient Transient { get; set; }
    }
        
        
    [Test(Description = "Inject transient on transient, test OnCreated")]
    public void OnCreateTestsTransientOnTransientTes() {
        PostInjectTransient.Created = 0;
        var singletons = new List<object>();
        var transients = new List<object>();
        var c = new Container();
        c.OnCreated += (lifetime, instance) => {
            if (lifetime == Lifetime.Singleton) {
                singletons.Add(instance);
            } else {
                transients.Add(instance);
            }
        };

        var di = c.CreateBuilder();
        di.Scan<TransientWithTransient>();
        di.Scan<PostInjectTransient>();
        di.Build();
            
        Assert.That(singletons.Count, Is.EqualTo(0));
        Assert.That(transients.Count, Is.EqualTo(0));
        Assert.That(PostInjectTransient.Created, Is.EqualTo(0));

        var s = c.Resolve<TransientWithTransient>();
        Assert.That(PostInjectTransient.Created, Is.EqualTo(1));
        Assert.That(s.Transient.Called, Is.EqualTo(1));
        Assert.That(singletons.Count, Is.EqualTo(0));
        Assert.That(transients.Count, Is.EqualTo(2));
        Assert.That(s, Is.EqualTo(transients[0]));
        Assert.That(s.Transient, Is.EqualTo(transients[1]));

    }

}