namespace nu
{
    using System;
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.Windsor;
    using Commands;
    using Model.ArgumentParsing;
    using Model.Package;
    using Model.Project;
    using Model.Project.Persistence;
    using Model.Project.Transformation;
    using Model.Template;
    using Utility;

    public class NuConfiguration
    {
        public static void Configure(IWindsorContainer container)
        {
            //allows us to take a dependency on arrays
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));

            //
            container.AddComponent<Dispatcher>("dispatcher");

            //argument parsing
            container.AddComponentWithLifestyle<IArgumentParser, ArgumentParser>("argumentParser", LifestyleType.Transient);
            container.AddComponentWithLifestyle<IArgumentMapFactory, ArgumentMapFactory>("argumentMapFactory", LifestyleType.Transient);

            //helper shims
            container.AddComponentWithLifestyle<IConsole, ConsoleHelper>("consoleHelper", LifestyleType.Transient);
            container.AddComponentWithLifestyle<IPath, PathAdapter>("pathAdapter", LifestyleType.Transient);
            container.AddComponentWithLifestyle<IFileSystem, FileSystem>("fileSystem", LifestyleType.Transient);

            //templating
            container.AddComponentWithLifestyle<ITemplateProcessor, NVelocityTemplateProcessor>("templateProcessor",
                                                                                                LifestyleType.Transient);

            //package repository
            container.AddComponent<IPackageRepository, LocalPackageRepository>("package.repository");

            //project stuff
            container.AddComponentWithLifestyle<IProjectManifestStore, XmlProjectManifestStore>("xmlProjectStore", LifestyleType.Transient);
            container.AddComponentWithLifestyle<IProjectManifestRepository, ProjectManifestRepository>("projectManifestRepository", LifestyleType.Transient);

            //default package commands
            container.AddComponent<ICommand, HelpCommand>("help");
            container.AddComponent<ICommand, NewProjectCommand>("project");
            SetupNewProject(container);
            container.AddComponent<ICommand, ListCommand>("list");
            container.AddComponent<ICommand, InjectCommand>("inject");
        }

        private static void SetupNewProject(IWindsorContainer container)
        {
            container.AddComponentWithLifestyle<ITransformationElement, FolderTransformationElement>(
                "folderTransformation", LifestyleType.Transient);
            container.AddComponentWithLifestyle<ITransformationElement, FileTransformationElement>(
                "fileTransformation", LifestyleType.Transient);
            container.AddComponentWithLifestyle<IProjectTransformationPipeline, ProjectTransformationPipeline>(
                "transformationPipeline", LifestyleType.Transient);
        }
    }

    public class ArrayResolver :
        ISubDependencyResolver
    {
        private readonly IKernel _kernel;

        public ArrayResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        #region ISubDependencyResolver Members

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                              DependencyModel dependency)
        {
            return null;
            //return _kernel.ResolveAll(dependency.TargetType.GetElementType(), null);
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                               DependencyModel dependency)
        {
            Type targetType = dependency.TargetType;
            return targetType != null &&
                   targetType.IsClass &&
                   targetType.IsArray &&
                   targetType.GetElementType().IsInterface;
        }

        #endregion
    }
}