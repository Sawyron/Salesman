﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfUI.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Pathfinders {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Pathfinders() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WpfUI.Resources.Pathfinders", typeof(Pathfinders).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Backtracking random search.
        /// </summary>
        internal static string BacktrackingRandomSearch {
            get {
                return ResourceManager.GetString("BacktrackingRandomSearch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Branch and bounds.
        /// </summary>
        internal static string BnB {
            get {
                return ResourceManager.GetString("BnB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dynamic.
        /// </summary>
        internal static string Dynamic {
            get {
                return ResourceManager.GetString("Dynamic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exhaustive search.
        /// </summary>
        internal static string Exhaustive {
            get {
                return ResourceManager.GetString("Exhaustive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Greedy.
        /// </summary>
        internal static string Greedy {
            get {
                return ResourceManager.GetString("Greedy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Random search.
        /// </summary>
        internal static string RandomSearch {
            get {
                return ResourceManager.GetString("RandomSearch", resourceCulture);
            }
        }
    }
}
