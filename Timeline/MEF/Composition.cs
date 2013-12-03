using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Reflection;
using System.ComponentModel;

namespace ShiningMeeting.MEF
{
    public static class Composition
    {
        #region Compose()

        /// <summary>
        /// 组合
        /// </summary>
        /// <param name="o">当前实例</param>
        public static void Compose(object o)
        {
            Compose(null, o);
        }

        private static void Compose(CompositionContainer cc, object o)
        {
            if (o == null)
                return;
            if (cc == null)
                cc = Container;

            var batch = new CompositionBatch();
            batch.AddPart(o);
            cc.Compose(batch);
        }

        #endregion

        #region GetInstance()
        /// <summary>
        /// 获取MEF导入对象单实例,若取子类型的实例,应带参数
        /// </summary>
        /// <typeparam name="T">获取单实例的类型,类型为导入类型</typeparam>
        /// <param name="type">子类的类型</param>
        /// <returns></returns>
        public static T GetInstance<T>(this Type type)
        {
            string typeName = AttributedModelServices.GetContractName(type);
            if (string.IsNullOrEmpty(typeName)) return default(T);

            T result = default(T);
            if (typeName == null)
            {
                result = Container.GetExportedValueOrDefault<T>();
            }
            else
            {
                foreach (var item in Container.GetExports<T>())
                {
                    if (GetContractName(item.Value.GetType()) == typeName) 
                    {
                        result = item.Value;
                        break;
                    }
                }
            }
            return result;
        }

        public static System.Collections.Generic.IEnumerable<Lazy<T>> GetList<T>(Type type = null) 
        {
            string typeName = AttributedModelServices.GetContractName(type);
            if (string.IsNullOrEmpty(typeName))
            {
                return Container.GetExports<T>();
            }
            else 
            {
                System.Collections.Generic.List<Lazy<T>> list = new System.Collections.Generic.List<Lazy<T>>();
                foreach (var item in Container.GetExports<T>())
                {
                    if (GetContractName(item.Value.GetType()) == typeName)
                    {
                        list.Add(item);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// 获取类型的契约名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetContractName(this Type type)
        {
            return AttributedModelServices.GetContractName(type);
        }

        /// <summary>
        /// 导入MEF单实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="instance">实例</param>
        public static void ExportInstance<T>(T instance) 
        {
            if (instance != null) 
            {
                Container.ComposeExportedValue(instance);
            }
        }
        #endregion

        #region Catalog, Container, CurrentContext, Register(), Reset()


        public static ServiceContext CurrentContext
        {
            get { return ContextCatalog.CurrentContext; }
            set { ContextCatalog.CurrentContext = value; }
        }

        static object locker = new object();

        /// <summary>
        /// 注册MEF反射集合
        /// </summary>
        /// <param name="assemblies"></param>
        public static void Register(params Assembly[] assemblies)
        {
            if (assemblies == null)
                return;
            foreach (var a in assemblies)
                if (a != null)
                    Catalog.Catalogs.Add(new AssemblyCatalog(a));
        }

        public static void Register(params Type[] types)
        {
            if (types == null || types.Length == 0)
                return;
            Catalog.Catalogs.Add(new TypeCatalog(types));
        }

        /// <summary>
        /// 重新注册
        /// </summary>
        public static void Reset()
        {
            lock (locker)
            {
                catalog = null;
                container = null;
                contextCatalog = null;
            }
        }

        public static AggregateCatalog Catalog
        {
            get
            {
                if (catalog == null)
                    lock (locker)
                        if (catalog == null)
                            catalog = new AggregateCatalog();
                return catalog;
            }
        }
        static AggregateCatalog catalog;

        static ContextCatalog ContextCatalog
        {
            get
            {
                if (contextCatalog == null)
                    lock (locker)
                        if (contextCatalog == null)
                            contextCatalog = new ContextCatalog(Catalog);
                return contextCatalog;
            }
        }
        static ContextCatalog contextCatalog;

        private static CompositionContainer Container
        {
            get
            {
                if (container == null)
                    lock (locker)
                        if (container == null)
                            container = new CompositionContainer(ContextCatalog, true);
                return container;
            }
        }
        static CompositionContainer container;

        #endregion
    }
}
