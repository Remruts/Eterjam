using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> {

	List<Tuple<T, float>> heap;

	public PriorityQueue(){
		heap = new List<Tuple<T, float>>();
	}

	public void Add(T obj, float priority){
		Tuple<T, float> tuple = new Tuple<T, float>(obj, priority);
		heap.Add(tuple);

		int i = heap.Count - 1;
		while(i > 0 && heap[(i-1)/2].item2 > priority){
			heap[i] = heap[(i-1)/2];
			i = (i-1)/2;
		}
		heap[i] = tuple;
	}

	void heapify(int i){
		int l = 2*i + 1;
		int r = 2*i + 2;
		int smallest;
		if (l < heap.Count && heap[l].item2 < heap[i].item2){
			smallest = l;
		} else {
			smallest = i;
		}
		if (r < heap.Count && heap[r].item2 < heap[smallest].item2){
			smallest = r;
		}
		if (smallest != i){
			//swap
			Tuple<T, float> temp = heap[i];
			heap[i] = heap[smallest];
			heap[smallest] = temp;

			heapify(smallest);
		}
	}

	public T Get(){
		// get result
		T result = heap[0].item1;

		// swap
		Tuple<T, float> temp = heap[0];
		heap[0] = heap[heap.Count-1];
		heap[heap.Count-1] = temp;

		// remove
		heap.RemoveAt(heap.Count-1);
		heapify(0);

		return result;
	}

	public bool Empty(){
		return (heap.Count == 0);
	}

	public void printQueue(){
		Debug.Log("Queue priorities:" + String.Join(" ",
    		heap.ConvertAll(i => i.item2.ToString()).ToArray()));
		Debug.Log("Queue contents:" + String.Join(" ",
	    	heap.ConvertAll(i => i.item1.ToString()).ToArray()));
	}
}
