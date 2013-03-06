﻿using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Testing;
using vlko.BlogModule.Tests.Model;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
    [TestClass]
    public class TimelinePagingTest : TimelinePagingBaseTest
    {
        public TimelinePagingTest()
            : base(new RavenDBTestProvider())
        {
        }

        [TestInitialize]
        public override void Init()
        {
            base.Init();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        public override void Get_half_half_items()
        {
            base.Get_half_half_items();
        }

        [TestMethod]
        public override void Get_only_first()
        {
            base.Get_only_first();
        }

        [TestMethod]
        public override void Get_only_second()
        {
            base.Get_only_second();
        }

        [TestMethod]
        public override void Get_more_first()
        {
            base.Get_more_first();
        }

        [TestMethod]
        public override void Get_more_first_middle_page()
        {
            base.Get_more_first_middle_page();
        }

        [TestMethod]
        public override void Get_more_second()
        {
            base.Get_more_second();
        }

        [TestMethod]
        public override void Get_more_second_middle_page()
        {
            base.Get_more_second_middle_page();
        }
    }

}
