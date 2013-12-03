using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;

namespace ShiningMeeting.MEF
{
    class ContextCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        readonly ComposablePartCatalog inner;

        public ContextCatalog(AggregateCatalog inner)
        {
            this.inner = inner;
            inner.Changed += (o, e) =>
            {
                Update();
                if (Changed != null)
                    Changed(this, e);
            };
            inner.Changing += (o, e) =>
            {
                if (Changing != null)
                    Changing(this, e);
            };
            Update();
        }

        public ServiceContext CurrentContext
        {
            get { return context; }
            set
            {
                switch (value)
                {
                    default:
                        throw new ArgumentOutOfRangeException();
                    case ServiceContext.Runtime:
                    case ServiceContext.DesignTime:
                    case ServiceContext.TestTime:
                        break;
                }
                if (value == context)
                    return;
                context = value;
                Update();
            }
        }
        ServiceContext context = ServiceContext.Runtime;

        void Update()
        {
            this.parts = (
                from ip in inner.Parts
                let fp = new FilteredPartDefinition(ip, CurrentContext)
                where fp.ExportDefinitions.Count() > 0
                select (ComposablePartDefinition)fp
            ).ToList();
        }

        public override IQueryable<ComposablePartDefinition> Parts { get { return parts.AsQueryable<ComposablePartDefinition>(); } }

        List<ComposablePartDefinition> parts;

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        class FilteredPartDefinition : ComposablePartDefinition
        {
            ComposablePartDefinition inner;
            List<ExportDefinition> filteredExports;

            public FilteredPartDefinition(ComposablePartDefinition inner, ServiceContext current)
            {
                this.inner = inner;
                filteredExports = (
                    from e in inner.ExportDefinitions
                    select e
                ).ToList();
            }

            public override ComposablePart CreatePart() { return inner.CreatePart(); }
            public override IEnumerable<ExportDefinition> ExportDefinitions { get { return filteredExports; } }
            public override IEnumerable<ImportDefinition> ImportDefinitions { get { return inner.ImportDefinitions; } }
            public override IDictionary<string, object> Metadata { get { return inner.Metadata; } }
        }
    }
}
