# CRMImporter
A data import library for dynamics crm

# Description
This is a simple to use data import library that tries to automatically cast data to the correct data types

# Building
Standard .net build. Can be built using either visual studio or msbuild

# Installing
This library is available through nuget.org and can be added to a project by installing the 'Wycliffeassociates.CRMImport' nuget package

# Components

## FieldMap
Used to build a mapping between one field and another.
`new FieldMap("<source-field>", "<destination-field>", new ValueConverter())`

The last argument is optional. It is used for an extension point for handling data transformation and manipulation and runs before the library tries to guess data formats. 
Any function that implements CRMImporter.IConverter can be used here. The library ships with a couple in CRMImporter.Converters most notabley LookupValue() which will 
needed to populate Lookup fields
## ImportMap
This is the main workhorse of the library.
```
    ImportMap map = new ImportMap("<entity>", new FieldMap("<source-key>", "<destination-key>"))
    {
        Mapping = new List<FieldMap>()
        {
            // Field mappings here
        }
    };

```

The constructor takes in the logical name of the entity you want to import into as well as a mapping that describes how the data should be matched up.
Since the matching is a FieldMap object you can transform it on the fly if you need to. Mapping needs to contain a List<FieldMap> of the mapping that you need.

To import the data call
`map.Import(<crm-org-service>, <data> , <callback-handler>) `

The organization service needs to be one that implements IOrganizationService, data needs to be a `List<Dictionary<string,object>>`. The callback handler is optional and can be used
to report about the progress of the import.

# Example usage 
The below example will import the list of dictionary data into the entity entity by using the key attribute to attempt to match to existing data
where new_keyfield matches. The data in field will be put into new_peid.
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