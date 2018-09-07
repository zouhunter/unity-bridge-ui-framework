# List View Framework #

The List View Framework is a set of core classes which  can be used as-is or extended to create dynamic, scrollable lists in Unity applications.  This repository tracks the development of the core scripts of the framework. An example Unity project is available [as its own repository](https://bitbucket.org/Unity-Technologies/list-view-framework) and also as a package on the Unity Asset Store. If you do not wish to contribute to the project, we recommend that you download and import the latest version on the Asset Store for your convenience.

This package is intended for experienced developers who want a starting-point for customization. The wiki contains in-depth explanations of the [Core Classes](https://bitbucket.org/Unity-Technologies/list-view-framework-core/wiki).

The code is set up to be customizable, but not "intelligent" or comprehensive. In other words, this is not a one-size-fits-all solution with lots of options and internal logic to adapt to the type of data set.  Instead, the idea is to extend the core classes into a number of list types which handle different requirements throughout your project.  In this way, we avoid a single, monolithic and complex script file which is hard to read, and at the same time, we can ensure that our lists aren't wasting CPU cycles on unnecessary branching, i.e. if(horizontal) or if(smoothScrolling).  At the same time, developers are free to create their own one-size-fits-all implementation if they have the need to switch options on-the-fly, or if for some other reason their use case demands it.

More details about the design and implementation of this framework on our [Unity Labs website](https://labs.unity.com/article/list-view-framework)

#Usage#
List view implementations will require at a minimum:

1. A GameObject with a ListViewController (or extension) component
2. At least one template prefab with a ListViewItem (or extension) component
3. A data source composed of ListViewData objects

#Requirements#
This project was developed on Unity 5.4, but could reasonably work on any version of Unity, since it doesn't rely on particularly new APIs.

#Forks#
Anyone in the community is welcome to create their own forks. Drop us a note to labs@unity3d.com if you find this package useful, we'd love to hear from you!