using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using TriggerBase = Microsoft.Xaml.Behaviors.TriggerBase;

namespace SisMaper.Views.Templates.MyInteractions
{
    public class Interaction
    {
        public Interaction()
        {

        }

        /// <summary>
        /// This property is used as the internal backing store for the public Triggers attached property.
        /// </summary>
        /// <remarks>
        /// This property is not exposed publicly. This forces clients to use the GetTriggers and SetTriggers methods to access the
        /// collection, ensuring the collection exists and is set before it is used.
        /// </remarks>
        private static readonly DependencyProperty MyTriggersProperty = DependencyProperty.RegisterAttached(
            "MyTriggers",
            typeof(TriggerCollection),
            typeof(Interaction),
            new FrameworkPropertyMetadata(new TriggerCollection(),
                new PropertyChangedCallback(OnTriggersChanged)));

        /// <summary>
        /// Gets the TriggerCollection containing the triggers associated with the specified object.
        /// </summary>
        /// <param name="obj">The object from which to retrieve the triggers.</param>
        /// <returns>A TriggerCollection containing the triggers associated with the specified object.</returns>
        public static TriggerCollection GetMyTriggers(DependencyObject obj)
        {
            TriggerCollection triggerCollection = (TriggerCollection) obj.GetValue(Interaction.MyTriggersProperty);
            if (triggerCollection == null)
            {
                triggerCollection = new TriggerCollection();
                obj.SetValue(Interaction.MyTriggersProperty, triggerCollection);
            }

            return triggerCollection;
        }

        /// <exception cref="InvalidOperationException">Cannot host the same TriggerCollection on more than one object at a time.</exception>
        private static void OnTriggersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TriggerCollection oldCollection = args.OldValue as TriggerCollection;
            TriggerCollection newCollection = args.NewValue as TriggerCollection;

            if (oldCollection != newCollection)
            {
                if (oldCollection != null && oldCollection.AssociatedObject != null)
                {
                    oldCollection.Detach();
                }

                if (newCollection != null && obj != null)
                {
                    if (((IAttachedObject) newCollection).AssociatedObject != null)
                    {
                        return;
                    }

                    newCollection.Attach(obj);
                }
            }
        }

        /// <summary>
        /// A helper function to take the place of FrameworkElement.IsLoaded, as this property is not available in Silverlight.
        /// </summary>
        /// <param name="element">The element of interest.</param>
        /// <returns>True if the element has been loaded; otherwise, False.</returns>
        internal static bool IsElementLoaded(FrameworkElement element)
        {
            return element.IsLoaded;
        }
    }

    public class TriggerCollection : Collection<TriggerBase>
    {
    internal DependencyObject AssociatedObject { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TriggerCollection"/> class.
    /// </summary>
    /// <remarks>Internal, because this should not be inherited outside this assembly.</remarks>
    internal TriggerCollection()
    {
    }

    /// <summary>
    /// Called immediately after the collection is attached to an AssociatedObject.
    /// </summary>
    protected void OnAttached()
    {
        foreach (TriggerBase trigger in this)
        {
            trigger.Attach(this.AssociatedObject);
        }
    }

    /// <summary>
    /// Called when the collection is being detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    protected void OnDetaching()
    {
        foreach (TriggerBase trigger in this)
        {
            trigger.Detach();
        }
    }

    /// <summary>
    /// Called when a new item is added to the collection.
    /// </summary>
    /// <param name="item">The new item.</param>
    internal void ItemAdded(TriggerBase item)
    {
        if (this.AssociatedObject != null)
        {
            item.Attach(this.AssociatedObject);
        }
    }

    /// <summary>
    /// Called when an item is removed from the collection.
    /// </summary>
    /// <param name="item">The removed item.</param>
    internal void ItemRemoved(TriggerBase item)
    {
        if (((IAttachedObject) item).AssociatedObject != null)
        {
            item.Detach();
        }
    }

    public void Detach()
    {
        AssociatedObject = null;
        OnDetaching();
    }

    public void Attach(DependencyObject dependencyObject)
    {
        if (dependencyObject != AssociatedObject)
        {
            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException();
            }
            AssociatedObject = dependencyObject;

            this.OnAttached();
        }
    }
    }
}