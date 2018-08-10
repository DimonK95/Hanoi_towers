using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Solver : MonoBehaviour
{
	[Range(0.1f, 4f)]
	public float TIMESTEP = 2f;  //скорость визуализации
	
	private const float X_OFFSET = 4f; 
	private const float Y_UP = 3.5f;
	private const float HEIGTH_DISK = 0.5f;
	
	public Text _input;
	public Text _message;
	public Text _buttonText;
	public Image _loadingImage;
	
	public GameObject[] disks;
	public Stack<GameObject>[] towers = new Stack<GameObject>[3];
	
	public void StartSolving()
	{
		idxAnswer = 0;
		answer.Clear();
		StopCoroutine(VisualSolving());
		DOTween.KillAll();
		for (int i = 0; i < towers.Length; i++)
			towers[i].Clear();
		for (int i = 0; i < disks.Length; i++)
		{
			disks[i].SetActive(false);
			disks[i].transform.position = new Vector2(-10000f, -10000f);
		}
		if (_message.text == "Решается")
			return;
		
		if (_input.text.Length == 0)
		{
			_message.text = "Введите количество дисков";
			return;
		}
		int cntDisks = int.Parse(_input.text);
		if (cntDisks <= 0)
		{
			_message.text = "Количество дисков должно быть больше нуля";
			return;
		}
		if (cntDisks > 10)
		{
			_message.text = "Количество дисков должно быть не больше 10";
			return;
		}

		_message.text = "Решается";
		_buttonText.text = "Стоп";
		
		for (int i = 0; i < cntDisks; i++)
		{
			disks[i].SetActive(true);
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(GetXposTower(0), Y_UP), -Vector2.up);
			disks[i].transform.position = new Vector2(GetXposTower(0), hit.collider.transform.position.y + HEIGTH_DISK);
			towers[0].Push(disks[i]);
		}
		Solve(cntDisks, 0, 1, 2);
		//foreach (var item in answer) print(item);
		StartCoroutine(VisualSolving());
	}

	void Awake()
	{
		for (int i = 0; i < towers.Length; i++)
		{
			towers[i] = new Stack<GameObject>();
		}
	}

	private List<ResultType> answer = new List<ResultType>();
	
	// перености n дисков с башни fromTower на башню toTower,
	// пользуясь башней auxTower как вспомогательной
	void Solve(int n, int fromTower, int auxTower, int toTower)
	{
		if (n == 1)
		{
			answer.Add(new ResultType(towers[fromTower].Peek(), fromTower, toTower));
			towers[toTower].Push(towers[fromTower].Pop());
			return;
		}
		Solve(n - 1, fromTower, toTower, auxTower);
		answer.Add(new ResultType(towers[fromTower].Peek(), fromTower, toTower));
		towers[toTower].Push(towers[fromTower].Pop());
		Solve(n - 1, auxTower, fromTower, toTower);
	}

	float GetXposTower(int towerNumber)
	{
		switch (towerNumber)
		{
			case 0: return -X_OFFSET;
			case 1: return 0;
			case 2: return X_OFFSET;
		}
		return -1;
	}

	private int idxAnswer = 0;
	IEnumerator VisualSolving()
	{
		while (true)
		{
			_loadingImage.fillAmount = (float)idxAnswer / answer.Count;
			if (idxAnswer >= answer.Count)
			{
				_message.text = answer.Count == 0 ? "" : "Решено";
				_buttonText.text = "Решить";
				yield break;
			}

			GameObject disk = answer[idxAnswer].Obj;
			int fromTower = answer[idxAnswer].From;
			int toTower = answer[idxAnswer].To;
			Sequence seq = DOTween.Sequence();
			seq.Append(	disk.transform.DOMoveY(Y_UP, TIMESTEP / 3f));
			seq.Append(	disk.transform.DOMoveX(GetXposTower(toTower), TIMESTEP / 3f));
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(GetXposTower(toTower), Y_UP), -Vector2.up);
			seq.Append(disk.transform.DOMoveY(hit.collider.transform.position.y + HEIGTH_DISK, TIMESTEP / 3f));
			Tween tween = seq;
			tween.Play();
			idxAnswer++;
			yield return tween.WaitForCompletion();
		}
	}
}
