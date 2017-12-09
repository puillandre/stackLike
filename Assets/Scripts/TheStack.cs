using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour
{
    /* Déclarez la variable publique scoreText type Text */

    /* Déclarez la variable publique endPanel de type GameObject*/

    /* Déclarez une variable publique prefab de type GameObject*/

    /* Déclarez la variable publique particle de type ParticleSystem */

    /* Déclarez la variable publique nbBoxes de type int*/
    
    
    /* Déclarez une chaine privée theStack de type GameObject */


    /* Déclarez la variable privée scoreCount de type int et initialisez la à 0 */

    /* Déclarez la variable privée stackIndex de type int et initialisez la à 0 */

    /* Déclarez la variable privée combo de type int et initialisez la à 0 */

    /* Déclarez la variable privée isMovingOnX de type booléen */

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;

    private bool isMovingOnX = true;
    private bool gameOver = false;

    private Vector2 stackBounds = new Vector2(BOUND_SIZE, BOUND_SIZE);
    private const float BOUND_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 1.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_BOUNDS_GAIN = 3;

    public int StackIndex
    {
        get { return stackIndex; }
        set { stackIndex = value; }
    }

    // Start est la fonction appelée au lancement du jeu
    private void Start()
    {
        SpawnBoxes();
    }

    /* Permet la création des pièces au démarrage */
    private void SpawnBoxes()
    {
        theStack = new GameObject[nbBoxes];
        for (int i = 0; i < nbBoxes; ++i)
        {
            theStack[i] = Instantiate(prefab, new Vector3(0, -i - 1, 0), Quaternion.identity, transform);
        }
        stackIndex = nbBoxes - 1;
    }

    // Update est la fonction appelée à chaque frame par le jeux. Essayez de coder cette fonction !
    void Update()
    {
        /* D'abord, on vérifie si le jeu est terminé, */
        {
            /* si oui, on arrête */
        }

        /* ensuite on regarde si le joueur clique ou appuie sur la barre espace (indice = Input....("Fire1")) */
        {
            /* Si le joueur a placé la pièce correctement */
            {
                /* On fait apparaitre une nouvelle pièce */

                /* On augmente le score */

                /* On affiche le score */
            }
            /* Sinon */
            {
                /* On termine le jeux et on quitte la fonction*/
            }
        }            
        /* Je peux maintenant bouger la pièce */

        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime); /* Je bouge la stack*/
    }

    /* Fonction permettant le mouvement de la pièce */
    private void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingOnX)
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUND_SIZE, scoreCount, secondaryPosition);
        else
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUND_SIZE);
    }

    /* Fonction permettant de créer une pièce */
    private void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex = stackIndex - 1;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;

        desiredPosition = (Vector3.down) * scoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
    }

  /* La fonction addCombo est appelée lorsque le jeu a detecté une pièce placée parfaitement */
	private void addCombo(Transform t)
	{
        /* On augmente la variable combo */
		
        /* si le combo est plus grand que la limite définie par COMBO_BOUNDS_GAIN */
		{
            /* On vérifie si la pièce actuelle n'est pas plus grande que la pièce de départ en x */
            {
                /* On augmente la taille de la pièce en x par le coefficient défini par STACK_BOUNDS_GAIN */
            }
            /* On vérifie si la pièce actuelle n'est pas plus grande que la pièce de départ en y */
            {
                /* On augmente la taille de la pièce en y par le coefficient défini par STACK_BOUNDS_GAIN */
            }
		}
		t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
	}

    /* Permet de créer les débris lors du découpage */
    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.GetComponentInChildren<RandomMaterial>().SetMaterialID(theStack[stackIndex].GetComponentInChildren<RandomMaterial>().getMaterialID());
    }

    /* Fonction permettant de placer la pièce et de la découper lorsqu'elle se déplace selon x*/
    private bool PlaceOnX(Transform t)
	{
		float deltaX = lastTilePosition.x - t.transform.position.x;
		if (Mathf.Abs(deltaX) > ERROR_MARGIN)
		{
			combo = 0;
			stackBounds.x -= Mathf.Abs(deltaX);
			if (stackBounds.x <= 0)
				return false;

			float middle = (lastTilePosition.x + t.localPosition.x) / 2;
			t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
			CreateRubble(
				new Vector3(t.position.x > 0 ? t.position.x + t.localScale.x / 2 : t.position.x - t.localScale.x / 2, t.position.y, t.position.z),
				new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)
			);
			t.localPosition = new Vector3(middle, scoreCount, lastTilePosition.z);
		}
		else
		{
			addCombo (t);
		}
		return (true);
	}

    /* Fonction permettant de placer la pièce et de la découper lorsqu'elle se déplace selon y*/
    private bool PlaceOnY(Transform t)
	{
		float deltaZ = lastTilePosition.z - t.transform.position.z;
		if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
		{
            combo = 0;
			stackBounds.y -= Mathf.Abs(deltaZ);
			if (stackBounds.y <= 0)
				return false;

			float middle = (lastTilePosition.z + t.localPosition.z) / 2;
			t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
			CreateRubble(
				new Vector3(t.position.x, t.position.y, t.position.z > 0 ? t.position.z + t.localScale.z / 2 : t.position.z - t.localScale.z / 2),
				new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))
			);
			t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle);
		}
		else
		{
			addCombo(t);
		}
		return (true);
	}


    /* PlaceTile est appelée lorsque le joueur clique pour poser la pièce */
    private bool PlaceTile()
    {
        /* On défini la position de la nouvelle piece dans une variable t */
        Transform t = theStack[stackIndex].transform;
        /* si la pièce bouge sur l'axe x */
        {
            /* On essaye de poser la pièce via l'axe x (indice, appel de fonction), */
            {
                /* s'il y a une erreur, on arrête la fonction en renvoyant une erreur */
            }

            /* On bouge à la nouvelle position */

            /* On indique qu'on arete de bouger sur X */

        }
        /* Sinon */
        {
            /* On essaye de poser la pièce via l'axe y */
            {
                /* s'il y a une erreur, on arrête la fonction en renvoyant une erreur */
            }
		
            /* On bouge à la nouvelle position */

            /* On indique qu'on arete de bouger sur X */

        }
        /* si on arrive ici la fonction c'est qu'on as pas rencontré de problème  */
    }

    /* La fonction EndGame est la fonction appelée pour la fin du jeu */
    private void EndGame()
    {
        /* Si le score est plus grand que le highScore indice: PlayerPrefs...("score") */
        {
            /* On enregistre le nouveau score dans le highscore */
        }
        /* On permet a la dernière pièce de tomber indice : theStack[stackIndex]... */

        /* On affiche l'écran de fin indice endPanel... */
        /* On indique qu'il y a GameOver sur le reste du jeu */
    }

    /* La fonction permet de gérer le changement de scéne sur Unity */
    public void OnButtonClick(string sceneName)
    {
        particle.Stop();
        SceneManager.LoadScene(sceneName);
    }
}
