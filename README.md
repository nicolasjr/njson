#NJson

NJson is an extremely simple library for Serializing and Deserializing JSON. It’s differential resides in the fact that it can deserialize JSONs into a Model Class, with support for many types of variables.

It can also create a JSON string, requiring only a model class as parameter. That model class can even have pointers to other classes.

As a limitation, NJson can not handle some Unity specific class (GameObject, Transform, Material, etc).

##Usage

NJson is really straightforward to use. It provides two methods:
* Serialize
  - Returns a string with the JSON generated based on a model class.
* Deserialize
  - Returns an instance to a template class with the values set according to received values on the JSON. 

It’s crucial for NJson to work properly that the model classes have properties with the exact same name as the provided JSON.

If the model provides properties that can not be found on the JSON, nothing will happen. The same happening if theres a value on the JSON and no corresponding property on the model.

##Samples

Samples can be found inside folder samples, that comes with the plugin package. 

