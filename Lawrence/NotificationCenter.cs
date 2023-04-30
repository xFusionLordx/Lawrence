﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Lawrence {
    // Abstract base class for notifications
    public abstract class Notification {
        // Property to store the notification name
        public string Name { get; }

        // Constructor for the Notification class
        protected Notification(string name) {
            Name = name;
        }
    }

    // Class representing a player connected notification, inherits from Notification
    public class PlayerJoinedNotification : Notification {
        // Property to store the player entity
        public Entity Entity { get; }

        // Constructor for the PlayerConnectedNotification class
        public PlayerJoinedNotification(int id, string username, Entity entity)
            : base("PlayerJoined") {
            Entity = entity;
        }
    }

    public class PrimaryUniverseChangedNotification : Notification {
        public Universe Universe;

        public PrimaryUniverseChangedNotification(Universe universe) : base("PrimaryUniverseChanged") {
            Universe = universe;
        }
    }

    public class TickNotification : Notification {
        public TickNotification() : base("Tick") { }
    }

    /// <summary>
    /// Class responsible for managing notification subscriptions and posting
    /// </summary>
    public class NotificationCenter {
        // Dictionary to store subscribers for each notification type
        private ConcurrentDictionary<string, List<Delegate>> _subscribers;

        /// <summary>
        /// Constructor for the NotificationCenter class
        /// </summary>
        public NotificationCenter() {
            _subscribers = new ConcurrentDictionary<string, List<Delegate>>();
        }

        /// <summary>
        /// Method to subscribe a callback to a specific notification type
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void Subscribe<T>(Action<T> callback) where T : Notification {
            string notificationName = typeof(T).Name;
            if (_subscribers.TryGetValue(notificationName, out var callbacks)) {
                callbacks.Add(callback);
            } else {
                _subscribers[notificationName] = new List<Delegate> { callback };
            }
        }

        /// <summary>
        /// Method to unsubscribe a callback from a specific notification type
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void Unsubscribe<T>(Action<T> callback) where T : Notification {
            string notificationName = typeof(T).Name;
            if (_subscribers.TryGetValue(notificationName, out var callbacks)) {
                callbacks.Remove(callback);
            }
        }

        /// <summary>
        /// Method to post a notification to all subscribers of the given type
        /// </summary>
        /// <param name="notification"></param>
        /// <typeparam name="T"></typeparam>
        public void Post<T>(T notification) where T : Notification {
            string notificationName = typeof(T).Name;
            if (_subscribers.TryGetValue(notificationName, out var callbacks)) {
                foreach (Action<T> callback in callbacks) {
                    callback(notification);
                }
            }
        }
    }
}