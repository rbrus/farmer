[<AutoOpen>]
module Farmer.Arm.Storage

open Farmer

type StorageAccount =
    { Name : ResourceName
      Location : Location
      Sku : StorageSku
      Containers : (string * string) list }
    interface IArmResource with
        member this.ResourceName = this.Name
        member this.JsonModel =
            {| ``type`` = "Microsoft.Storage/storageAccounts"
               sku = {| name = this.Sku.ArmValue |}
               kind = "StorageV2"
               name = this.Name.Value
               apiVersion = "2018-07-01"
               location = this.Location.ArmValue
               resources = [
                   for (name, access) in this.Containers do
                    {| ``type`` = "blobServices/containers"
                       apiVersion = "2018-03-01-preview"
                       name = "default/" + name
                       dependsOn = [ this.Name.Value ]
                       properties = {| publicAccess = access |}
                    |}
               ]
            |} :> _