using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity6Sample;
using UnityEditor.VersionControl;

public class Test1 {
    [Test]
    public void Test1SimplePasses() {

    }

    MainScreenController controller;
    MainScreenModel model;
    // MainScreenView view;

    [SetUp]
    public void SetUp() {
        // view = Substitute.For<MainScreenView>();
        model = Substitute.For<MainScreenModel>();
        
        // Assert.That(view, Is.Not.Null);
        Assert.That(model, Is.Not.Null);

        model.Products.Returns(returnThis: new ObservableList<EpicProduct>());
        Assert.That(model.Products, Is.Not.Null);

        // controller = new MainScreenController();
    }
}