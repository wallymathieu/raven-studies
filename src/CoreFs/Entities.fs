namespace SomeBasicRavenApp.Core.Entities
open System

type Order = {mutable Id:string;mutable OrderDate:DateTime;mutable CustomerId:string;Products:List<string>;mutable Version:int }
and Customer= {mutable Id:string;mutable Firstname:string;mutable Lastname:string;Orders:List<Order>;mutable Version:int}
and Product= {mutable Id:string;mutable Cost:float;mutable Name:string;Products: List<string>;mutable Version:int}
