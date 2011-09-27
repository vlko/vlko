using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using vlko.core.InversionOfControl;
using vlko.core.NH.Repository;
using vlko.core.Testing;

namespace vlko.core.NH.Testing
{
	public class NHTestProvider : ITestProvider
	{
		protected Configuration Configuration;

		public void SetUp()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.Core.NH"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));
			Configuration = new Configuration()
				.SetProperty("dialect", "NHibernate.Dialect.SQLiteDialect")
				.SetProperty("connection.driver_class", "NHibernate.Driver.SQLite20Driver")
				.SetProperty("connection.connection_string", "Data Source=:memory:")
				//.SetProperty("dialect", "NHibernate.Dialect.MsSql2008Dialect")
				//.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver")
				//.SetProperty("connection.connection_string", @"Data Source=.\SQL2008;Initial Catalog=test;Integrated Security=True;Pooling=False")
				.SetProperty("connection.provider", typeof(InMemoryConnectionProvider).AssemblyQualifiedName)
				.SetProperty("use_proxy_validator", "false");
				//.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");

			ConfigureMapping(Configuration);

			SchemaMetadataUpdater.QuoteTableAndColumns(Configuration);

			var schema = new SchemaExport(Configuration);
			schema.Create(false, true);
		}

		public void TearDown()
		{
			SessionFactory.SessionFactoryInstance.Close();
			InMemoryConnectionProvider.Restart();
		}

		/// <summary>
		/// Configures the mapping.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public virtual void ConfigureMapping(Configuration configuration)
		{
			DbInit.InitMappings(configuration);
		}

		/// <summary>
		/// Gets the mapping types.
		/// </summary>
		/// <returns>Types for mapping</returns>
		public virtual Type[] GetMappingTypes()
		{
			return new Type[] { };
		}

		public void WaitForIndexing()
		{
			// we have sql db, no wait to finish indexing is needed
		}

		public void Create<T>(T model) where T : class
		{
			SessionFactory<T>.Create(model);
		}

		public T GetById<T>(object id) where T : class
		{
			return SessionFactory<T>.FindByPrimaryKey(id);
		}

		public IEnumerable<T> FindAll<T>() where T : class
		{
			return SessionFactory<T>.FindAll();
		}

		public int Count<T>() where T : class
		{
			return SessionFactory<T>.Count();
		}

		public IQueryable<T> AsQueryable<T>() where T : class
		{
			return SessionFactory<T>.Queryable;
		}
	}
}
