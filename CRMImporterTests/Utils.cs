using System.Reflection;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Crud.FakeMessageExecutors;
using FakeXrmEasy.Middleware.Messages;

namespace CRMImporterTests;

public static class Utils
{
    public static IXrmFakedContext GetContext()
    {
        return MiddlewareBuilder
            .New()
            .AddCrud()
            .AddFakeMessageExecutors(Assembly.GetAssembly(typeof(RetrieveRequestExecutor)))
            .AddFakeMessageExecutors(Assembly.GetAssembly(typeof(UpdateRequestExecutor)))
            .UseMessages()
            .SetLicense(FakeXrmEasyLicense.RPL_1_5)
            .Build();
    }
}