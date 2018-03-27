# CRMImporter
A data import library for dynamics crm

# Description
This is a simple to use data import library that tries to automatically cast data to the correct data types

# Building
Standard .net build. Can be built using either visual studio or msbuild

# Installing
This library is available through nuget.org and can be added to a project by installing the 'Wycliffeassociates.CRMImport' nuget package

# Example usage 
The below example will import the dictionary data into the entity entity by using the 
```
CrmServiceClient service = new CrmServiceClient("connection string");

ImportMap map = new ImportMap("entity", new FieldMap("key", "new_keyfield"))
{
    Mapping = new List<FieldMap>()
    {
        new FieldMap("field", "new_peid"),
    }
};
List<Dictionary<string, object>> data = new List<Dictionary<string, object>>(){
    new Dictionary<string, object>(){
        ["field"] = "data",
        ["key"] = 1
    }
};
map.Import(service, data, (int current, int total) =>
{
    Console.WriteLine($"Progress: {current}/{total}");
});
```