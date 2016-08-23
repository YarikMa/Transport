﻿using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Transport.Aca3.Helpers;
using Transport.Aca3.Models;

namespace Transport.Aca3.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private readonly Lazy<RelayCommand> _buildMap;
        private readonly DataSource _dataSource;

        public MapViewModel(DataSource dataSource)
        {
            _dataSource = dataSource;
            _buildMap = new Lazy<RelayCommand>(() => new RelayCommand(InvokeBuildMap));
        }

        private void InvokeBuildMap()
        {
            double zoom = 10;

            var nodes = new List<NodeViewModel>();

            for (var i = 0; i < _dataSource.NodesCoords.Count; i++)
            {
                var nodeCoords = _dataSource.NodesCoords[i];
                var node = new NodeViewModel
                {
                    Center = new Point(nodeCoords.X * zoom, nodeCoords.Y * zoom),
                    Name = i.ToString()
                };

                nodes.Add(node);
            }

            var edges = new List<EdgeViewModel>();

            for (var i = 0; i < _dataSource.AdjacencyMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < _dataSource.AdjacencyMatrix.GetLength(0); j++)
                {
                    if (Math.Abs(_dataSource.AdjacencyMatrix[i,j]) < Constants.Tolerance) continue;
                    var edge = new EdgeViewModel
                    {
                        Point1 = nodes[i].Center,
                        Point2 = nodes[j].Center
                    };

                    edges.Add(edge);
                }
            }

            nodes.ForEach(n => VisualItems.Add(n));
            edges.ForEach(e => VisualItems.Add(e));

            IsItemsLoaded = true;
        }

        public ICommand BuildMapCommand => _buildMap.Value;

        public ObservableCollection<VisualItemViewModel> VisualItems { get; } =
            new ObservableCollection<VisualItemViewModel>();

        private bool _isItemsLoaded;
        public bool IsItemsLoaded
        {
            get { return _isItemsLoaded; }
            set { Set(ref _isItemsLoaded, value); }
        }
    }
}