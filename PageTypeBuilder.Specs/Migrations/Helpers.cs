using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Specs.Helpers.Fakes;
using Refraction;

namespace PageTypeBuilder.Specs.Migrations
{
    public class Helpers
    {
        public class when_called_with_name_of_existing_page_type
        {
            static Migration migration;
            static string pageTypeName = "MyPageType";
            static IPageTypeRepository pageTypeRepository;

            Establish context = () =>
                {
                    var pageDefinitionRepository = new InMemoryPageDefinitionRepository();
                    var pageType = new FakePageType(pageDefinitionRepository);
                    pageType.Name = pageTypeName;

                    pageTypeRepository = new InMemoryPageTypeRepository(pageDefinitionRepository);
                    pageTypeRepository.Save(pageType);

                    var assembly = Create.Assembly(with =>
                        with.Class("Migration1")
                        .Inheriting<Migration>()
                        .Constructor(x =>
                            x.Parameter<IPageTypeRepository>("pageTypeRepository")
                             .PassToBase("pageTypeRepository")
                             .Parameter<IPageDefinitionRepository>("pageDefinitionRepository")
                             .PassToBase("pageDefinitionRepository")
                             .Parameter<ITabDefinitionRepository>("tabDefinitionRepository")
                             .PassToBase("tabDefinitionRepository"))
                        .PublicMethod(x =>
                        x.Named("Execute")
                        .IsOverride()
                        .Body("PageType(\"" + pageTypeName + "\").Delete();")));

                    migration = (Migration) assembly.GetTypeInstance("Migration1",
                        pageTypeRepository, pageDefinitionRepository, new TabDefinitionRepository());
                };

            Because of = () => migration.Execute();

            It should_delete_the_page_type_with_that_name
                = () => pageTypeRepository.Load(pageTypeName).ShouldBeNull();
        }

        public class when_no_page_type_matches_by_name
        {
            static Migration migration;
            static string pageTypeName = "MyPageType";
            static IPageTypeRepository pageTypeRepository;
            static Exception thrownException;

            Establish context = () =>
            {
                var pageDefinitionRepository = new InMemoryPageDefinitionRepository();
                pageTypeRepository = new InMemoryPageTypeRepository(pageDefinitionRepository);

                var assembly = Create.Assembly(with =>
                    with.Class("Migration1")
                    .Inheriting<Migration>()
                    .Constructor(x =>
                        x.Parameter<IPageTypeRepository>("pageTypeRepository")
                         .PassToBase("pageTypeRepository")
                         .Parameter<IPageDefinitionRepository>("pageDefinitionRepository")
                         .PassToBase("pageDefinitionRepository")
                         .Parameter<ITabDefinitionRepository>("tabDefinitionRepository")
                         .PassToBase("tabDefinitionRepository"))
                    .PublicMethod(x =>
                    x.Named("Execute")
                    .IsOverride()
                    .Body("PageType(\"" + pageTypeName + "\").Delete();")));

                migration = (Migration)assembly.GetTypeInstance("Migration1",
                    pageTypeRepository, pageDefinitionRepository, new TabDefinitionRepository());
            };

            Because of = () => thrownException = Catch.Exception(() => migration.Execute());

            It should_not_throw_an_exception
                = () => thrownException.ShouldBeNull();
        }
    }
}
