namespace SomeBasicRavenApp.Core.Internal

// Using code from https://github.com/NewtonsoftJsonExt/Saithe

type ValueTypeMapping<'T>() =
    let t = typeof<'T>
    let properties = t.GetProperties()
                    |>Array.toList
    let fields = t.GetFields()
                    |>Array.filter (fun f->not f.IsStatic)
                    |>Array.toList
    let (getValue, propertyType)=
        match properties, fields with
        | [], [f]->(f.GetValue,f.FieldType)
        | [p], []->((fun o->p.GetValue(o,null)), p.PropertyType)
        | _, _ -> 
            failwithf "Should have a single instance field or property"

    let ctor = t.GetConstructor([|propertyType|])

    member this.PropertyType = propertyType

    member this.Parse(value:obj) =
        ctor.Invoke([|value|])

    member this.ToRaw(value) : obj=
        getValue(value)

open System.ComponentModel
type public ValueTypeConverter<'T>() = 
    inherit TypeConverter()
    let mapping = ValueTypeMapping<'T>()

    override this.CanConvertFrom(context, sourceType) = 
        if (sourceType = mapping.PropertyType) then true
        else base.CanConvertFrom(context, sourceType)
    
    override this.ConvertFrom(context, culture, value) = 
        if (mapping.PropertyType = value.GetType()) then mapping.Parse(value)
        else base.ConvertFrom(context, culture, value)
    
    override this.ConvertTo(context, culture, value, destinationType) = 
        if (mapping.PropertyType =destinationType) then mapping.ToRaw(value)
        else base.ConvertTo(context, culture, value, destinationType)

open Raven.Imports.Newtonsoft.Json
type public ValueTypeJsonConverter<'T>() = 
    inherit JsonConverter()
    let mapping = ValueTypeMapping<'T>()
    let t = typeof<'T>

    override this.CanConvert(objectType) = objectType = t
    
    override this.ReadJson(reader, objectType, existingValue, serializer) = 
        if (objectType = t) then 
            let v = serializer.Deserialize(reader, mapping.PropertyType)
            mapping.Parse(v)
        else 
            //base.ReadJson(reader, objectType, existingValue, serializer)
            failwithf "Cant handle type %s, expects %s" (objectType.Name) (t.Name)
    
    override this.WriteJson(writer, value, serializer) = 
        if (value :? 'T) then writer.WriteValue(mapping.ToRaw(value :?> 'T))
        else
            //base.WriteJson(writer, value, serializer)
            failwithf "Cant handle type %s, expects %s" (value.GetType().Name) (t.Name)