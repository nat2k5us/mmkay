namespace DataRepositoryTests.Repository
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Core.Common.Contracts;
    using Core.Common.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Transit.Business.Bootstrapper;
    using Transit.Business.Entities;
    using Transit.Data.Repositoryinterfaces;

    [TestClass]
    public class SignRepositoryTests
    {
        [TestMethod]
        public void CreateSignMockRepositoryFactoryTest()
        {
            var signs = new List<Sign>
                            {
                                new Sign { SignId = 1, AccountId = 101 },
                                new Sign { SignId = 2, AccountId = 102 }
                            };
            var mockSignRepository = new Mock<IDataRepositoryFactory>();
            mockSignRepository.Setup(obj => obj.GetDataRepository<ISignRepository>().Get()).Returns(signs);
            var repositoryFactoryTestClass = new RepositoryFactoryTestClass(mockSignRepository.Object);
            var result = repositoryFactoryTestClass.GetSigns();
            Assert.IsTrue(signs == result);
        }

        [TestMethod]
        public void CreateSignMockRepositoryTest()
        {
            var signs = new List<Sign>
                            {
                                new Sign { SignId = 1, AccountId = 101 },
                                new Sign { SignId = 2, AccountId = 102 }
                            };
            var mockSignRepository = new Mock<ISignRepository>();
            mockSignRepository.Setup(obj => obj.Get()).Returns(signs);
            var signRepositoryDiTestClass = new SignRepositoryDiTestClass(mockSignRepository.Object);
            var result = signRepositoryDiTestClass.GetSigns();
            Assert.IsTrue(signs == result);
        }

        [TestMethod]
        public void CreateSignRepositoryFactoryTest()
        {
            var repositoryFactoryTestClass = new RepositoryFactoryTestClass();
            var signs = repositoryFactoryTestClass.GetSigns();
            Assert.IsTrue(signs != null);
        }

        [TestMethod]
        public void CreateSignRepositoryTest()
        {
            var repositoryDiTestClass = new SignRepositoryDiTestClass();
            var signs = repositoryDiTestClass.GetSigns();
            Assert.IsNotNull(signs);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ObjectBase.Container = MefLoader.Init();
        }
    }

    public class RepositoryFactoryTestClass
    {
        [Import]
        private IDataRepositoryFactory dataRepositoryFactory;

        public RepositoryFactoryTestClass()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public RepositoryFactoryTestClass(IDataRepositoryFactory dataRepositoryFactory)
        {
            this.dataRepositoryFactory = dataRepositoryFactory;
        }

        public IEnumerable<Sign> GetSigns()
        {
            var signRepository = this.dataRepositoryFactory.GetDataRepository<ISignRepository>();
            var signs = signRepository.Get();
            return signs;
        }
    }

    public class SignRepositoryDiTestClass
    {
        [Import]
        private ISignRepository signRepository;

        public SignRepositoryDiTestClass()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public SignRepositoryDiTestClass(ISignRepository signRepository)
        {
            this.signRepository = signRepository;
        }

        public IEnumerable<Sign> GetSigns()
        {
            var signs = this.signRepository.Get();
            return signs;
        }
    }
}