using System.Diagnostics;

namespace MatriceLib;

public class Matrice
{
    private int[] 
        Shape { get;}
    private double[][] Matrix { get; }

    /**
     *  Attribut permettant de créer une sous matrice en la remplissant par les lignes de la matrice courante dont  les index sont spécifiés dans le tableau index
     *  index: tableau contenant les indices des lignes à extraire
     *  T: variable qui permet de spécifier le nombre de thread ou processeur logique qui sera utilisé pour extraire les lignes de la matrice courante
     *  exemple: Matrice mat=new([
     *                              [1,2,3],
     *                              [4,5,6],
     *                              [7,8,9]]); 
     *                              console.WriteLine(mat[[0,2]]);=>[
     *                                                                [1,2,3],
     *                                                                [7,8,9]]
     */
    public Matrice this[int[] index,int T=1]
    {
        get{
            double[][] result=new double[index.Length][];
            if (T>1)
            {

                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < index.Length; i += T)
                    {
                        if(index[i]<Shape[0])
                            result[i]=Matrix[index[i]];
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for(int i=0;i<index.Length;i++)
                {
                    if (index[i] < Shape[0])
                    {
                        result[i]=Matrix[index[i]];
                    }
                }
            }
            return new Matrice(result);
        }
    }

    /**
     *  Attribut permettant de créer une sous matrice en la remplissant par les lignes et colonnes de la matrice courante dont  les index sont spécifiés dans le tableau index
     *  index: tableau contenant les indices des lignes et colonnes à extraire
     *  T: variable qui permet de spécifier le nombre de thread ou processeur logique qui sera utilisé pour extraire les lignes et colonnes de la matrice courante
     *  exemple: Matrice mat=new([
     *                  [1,2,3],
     *                  [4,5,6],
     *                  [7,8,9]]); 
     *                  console.WriteLine(mat[
     *                                         [0,2],
     *                                         [0,2]]);=>[
     *                                                     [1,3],
     *                                                     [7,9]]
     */
    public Matrice this[int[][] index,int T=1]
    {
        get{
            int[] rowIndex=index[0];
            int[] colIndex=index[1];
            double[][] result=new double[rowIndex.Length][];
            if (T>1)
            {

                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < rowIndex.Length; i += T)
                    {
                        if (i<Shape[0])
                        {
                            result[i] = new double[colIndex.Length];
                            for (int j = 0; j < colIndex.Length; j++)
                            {
                                if (j<Shape[1])
                                {
                                    result[i][j]=Matrix[rowIndex[i]][colIndex[j]];
                                }
                            }
                        }
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < rowIndex.Length; i++)
                {
                    if (i<Shape[0])
                    {
                        result[i] = new double[colIndex.Length];
                        for (int j = 0; j < colIndex.Length; j++)
                        {
                            if (j<Shape[1])
                            {
                                result[i][j]=Matrix[rowIndex[i]][colIndex[j]];
                            }
                        }
                    }
                }
            }
            return new Matrice(result);
        }
    }


    public override int GetHashCode()
    {
        return Shape[0]*Shape[1];
    }

    /**
     *  fonction pour vérifier l'égalité de deux matrices
     *
     */
    public override bool Equals(object? obj)
    {
        if (obj is Matrice matrice)
        {
            if (matrice.Shape[0]==this.Shape[0] && matrice.Shape[1]==this.Shape[1])
            {
                for (var i = 0; i < matrice.Shape[0]; i++)
                {
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (Math.Abs(matrice.Matrix[i][j] - this.Matrix[i][j])!=0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        return false;
    }

    /**
     *  fonction pour afficher les coefficients de la matricielle sous forme de chaine de caractère
     *  Exemple: Matrice mat=new([[1,2,3],[4,5,6],[7,8,9]]);
     *  Console.WriteLine(mat);=>[[1,2,3],[4,5,6],[7,8,9]]
     */
    public override string ToString()
    {
        string result = "[ ";
        for (var i = 0; i < Shape[0]; i++)
        {
            result += " [";
            for (var j = 0; j < Shape[1]; j++)
            {
                result += $"{Matrix[i][j]},";
            }
            result = result.Remove(result.Length - 1);
            result += "],\n";
        }
        result = result.Remove(result.Length - 1);
        result += "]";
        return $"{result}";
    }

    /**
     *  Fonction pour calculer la Trace de la matrice courante
     *
     */
    public double Trace()
    {
        if (Shape[0] == Shape[1])
        {
            double som = 0.0;
            for (int i = 0; i < Shape[0]; i++)
            {
                som += Matrix[i][i];
            }
            return som;
        }
        else
        {
            throw new Exception("Matrix must be square");
        }
    }

    /**
     *  opérateur pour facilité l'addition matricielle
     *
     */
    public static Matrice operator+(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]+matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité la soustraction matricielle
     *
     */
    public static Matrice operator-(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]-matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }


    /**
    *  opérateur pour avoir l'opposé des coefficients d'une matricielle
    *
    */
    public static Matrice operator -(Matrice matrice)
    {
         
        double[][] tmp = new double[matrice.
                Shape[0]][];

        for (var i = 0; i < matrice.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice.
                    Shape[1]];
            for (var j = 0; j < matrice.
                    Shape[1]; j++)
            {
                tmp[i][j] = (-1)*matrice.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }



    /**
     *  opérateur pour facilité le produit d'Hadamard
     *
     */
    public static Matrice operator*(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]*matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité le produit d'Hadamard
     *
     */
    public static Matrice operator/(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]/matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }

    /**
     *  opérateur pour facilité la multiplication des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator*(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]*=cte;
            }
        }

        return new Matrice(tmp);
    }

    /**
     *  Fonction pour récupérer les coefficients uniques contenu dans un tableau
     *
     */
    public static double[] Unique(double[] array)
    {
        List<double> list = new();
        for (int i = 0; i < array.Length; i++)
        {
            if (!list.Contains(array[i]))
            {
                list.Add(array[i]);
            }
        }
        return [..list];
    }

    /**
     *  Fonction pour générer la matrice identité
     *  Exemple: Matrice mat=Matrice.Identity(3); 
     *  Console.WriteLine(mat);=>[[1,0,0],[0,1,0],[0,0,1]]
     */
    public static Matrice Identity(int shape,int T=1)
    {  
        Matrice matrice = new([shape,shape]);
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < matrice.Shape[0]; i+=T)
                {
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (i == j)
                        {
                            matrice.Matrix[i][j] = 1.0;
                        }
                        else
                        {
                            matrice.Matrix[i][j] = 0.0;
                        }
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    if (i == j)
                    {
                        matrice.Matrix[i][j] = 1.0;
                    }
                    else
                    {
                        matrice.Matrix[i][j] = 0.0;
                    }
                }
            }
        }
        return matrice;
    }

    /**
     *  opérateur pour facilité le produit matrice et vecteur
     *
     */
    public static double[] operator *(Matrice matrice, double[] array)
    {
        if (array.Length == matrice.Shape[1])
        {
            double[] result = new double[matrice.Shape[0]];
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                double som = 0.0;
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    som += matrice.Matrix[i][j] * array[j];
                }
                result[i] = som;
            }
            return result;
        }
        else
        {
            throw new Exception("incompatible size");
        }
    }

    /**
     *  opérateur pour facilité l'addition des coéfficients d'une matrice avec une constante
     *  
     */
    public static Matrice operator+(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]+=cte;
            }
        }

        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator-(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]-=cte;
            }
        }

        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator/(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]/=cte;
            }
        }

        return new Matrice(tmp);
    }

    
    /**
     *  fonction pour vérifier l'égalité de deux matrices
     *
     */
    public static bool operator ==(Matrice matrice1, Matrice matrice2)
    {
        if (matrice1.Shape[0]==matrice2.Shape[0] && matrice1.Shape[1]==matrice2.Shape[1])
        {
            for (var i = 0; i < matrice1.Shape[0]; i++)
            {
                for (var j = 0; j < matrice1.Shape[1]; j++)
                {
                    if (Math.Abs(matrice1.Matrix[i][j] - matrice2.Matrix[i][j])!=0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }
    
    /**
     *  fonction pour vérifier si deux matrices ont au moins un coefficients différent
     *
     */
    public static bool operator !=(Matrice matrice1, Matrice matrice2)
    {
        if (matrice1.Shape[0]==matrice2.Shape[0] && matrice1.Shape[1]==matrice2.Shape[1])
        {
            for (var i = 0; i < matrice1.Shape[0]; i++)
            {
                for (var j = 0; j < matrice1.Shape[1]; j++)
                {
                    if (Math.Abs(matrice1.Matrix[i][j] - matrice2.Matrix[i][j])!=0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    /**
     *  Fonction faire le produit matriciel
     *  matrice1: la matrice avec laquelle on veut faire le produit avec la matrice courante
     *  T: nombre de thread ou processeur logique qui sera utilisé pour calculer le produit matriciel
     */
    public Matrice Dot(Matrice matrice1,int T=1)
    {
        if (matrice1.Shape[1]!=this.Shape[0])
        {
            throw new Exception("colonne number of matrice1 should be the same than row number of current matrice ");
        }
        double[][] tmp = new double[this.Shape[0]][];
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (var i = t; i < matrice1.
                 Shape[0]; i+=T)
                {
                    tmp[i] = new double[this.
                        Shape[1]];
                    double som = 0;
                    for (var j = 0; j < this.
                             Shape[1]; j++)
                    {
                        for (var k = 0; k < this.Shape[0]; k++)
                        {
                            som += matrice1.Matrix[i][k] * this.Matrix[k][j];
                        }
                        tmp[i][j] = som;
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (var i = 0; i < matrice1.
                 Shape[0]; i++)
            {
                tmp[i] = new double[this.
                    Shape[1]];
                double som = 0;
                for (var j = 0; j < this.
                         Shape[1]; j++)
                {
                    for (var k = 0; k < this.Shape[0]; k++)
                    {
                        som += matrice1.Matrix[i][k] * this.Matrix[k][j];
                    }
                    tmp[i][j] = som;
                }
            }
        }
        return new Matrice(tmp);
    }

    /**
     * Fonction pour faire le produit matrice-vecteur
     * matrice: la matrice avec laquelle on veut faire le produit
     * array: le vecteur avec lequel on veut faire le produit
     * T: nombre de thread ou processeur logique qui sera utilisé pour calculer le produit matrice-vecteur
     */
    public static double[] Dot(Matrice matrice, double[] array,int T=1)
    {
        if (array.Length == matrice.Shape[1])
        {
            double[] result = new double[matrice.Shape[0]];
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < matrice.Shape[0]; i+=T)
                    {
                        double som = 0.0;
                        for (int j = 0; j < matrice.Shape[1]; j++)
                        {
                            som += matrice.Matrix[i][j] * array[j];
                        }
                        result[i] = som;
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    double som = 0.0;
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        som += matrice.Matrix[i][j] * array[j];
                    }
                    result[i] = som;
                }
            }
            return result;
        }
        else
        {
            throw new Exception("incompatible size");
        }
    }

    /**
     * Fonction pour calculer la transposée d'une matrice
     * T: nombre de thread ou processeur logique qui sera utilisé pour calculer la transposée de la matrice courante
     */
    public Matrice Transpose(int T=1)
    { 
        double[][] matrix = new double[Shape[1]][];
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (var i = t; i < Shape[1]; i+=T)
                {
                    matrix[i] = new double[Shape[0]];
                    for (var j = 0; j < matrix[i].Length; j++)
                    {

                        matrix[i][j] = Matrix[j][i];

                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (var i = 0; i < Shape[1]; i++)
            {
                matrix[i] = new double[Shape[0]];
                for (var j = 0; j < matrix[i].Length; j++)
                {

                    matrix[i][j] = Matrix[j][i];

                }
            }
        }
        return new Matrice(matrix);
    }

    public Matrice(double[][] matrix)
    {
        Matrix = matrix;
        Shape=[matrix.Length,matrix[0].Length];
    }
    
    public Matrice(List<List<double>> matrix)
    {
        Matrix = new double[matrix.Count][];
        for (var i = 0; i < matrix.Count; i++)
        {
            Matrix[i] = [..matrix[i]];
        }
        Shape=[matrix.Count,matrix[0].Count];
    }

    public Matrice(int[] shape)
    {
        
        Shape = shape;
        Matrix = new double[
                Shape[0]][];
        for (var i = 0; i < 
                Shape[0]; i++)
        {
            Matrix[i] = new double[
                    Shape[1]];
        }
    }

    public Matrice(double[] array,int[] shape)
    {
        Shape=shape;
        Matrix=new double[
                Shape[0]][];
        var k = 0;
        for (var i = 0; i < 
                Shape[0]; i++)
        {
            Matrix[i] = new double[
                    Shape[0]];
            for (var j = 0; j < 
                    Shape[1]; j++)
            {
                Matrix[i][j] = array[k];
                k++;
            }
        }
    }

    public Matrice(List<double> array,int[] shape)
    {
        Shape=shape;
        Matrix=new double[
                Shape[0]][];
        var k = 0;
        for (var i = 0; i < 
                Shape[0]; i++)
        {
            Matrix[i] = new double[
                    Shape[0]];
            for (var j = 0; j < 
                    Shape[1]; j++)
            {
                Matrix[i][j] = array[k];
                k++;
            }
        }
    }

    /**
     * Fonction pour calculer le déterminant d'une matrice
     * matrice: la matrice dont on veut calculer le déterminant
     * j: l'indice de la colonne à partir de laquelle on veut calculer le déterminant
     */
    public static double Determinant(double[][] matrice, int j = 0)
    {
        if (matrice.Length == 2 && matrice[0].Length == 2)
        {
            return matrice[0][0] * matrice[1][1] - matrice[1][0] * matrice[0][1];
        }
        else
        {
            double deter = 0.0;
            for (int i = j; i < matrice[0].Length; i++)
            {
                deter += Math.Pow(-1.0, (double)i) * matrice[0][i] * Determinant(ExtractCoMatrix(matrice, i), i);
            }
            return deter;
        }
    }

    /**
     * Fonction pour cloner ou créer une copie de la matrice courante
     */
    public Matrice Copy() => new(Matrix);

    /**
     * Fonction pour calculer la matrice d'adjacence de la matrice courante
     *  T: nombre de thread ou processeur logique qui sera utilisé pour calculer la matrice d'adjacence
     */
    public Matrice Adjacence(int T=1)
    {
        Matrice mat = new(Shape);
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < mat.Shape[0]; i+=T)
                {
                    for (int j = 0; j < mat.Shape[1]; j++)
                    {
                        mat.Matrix[i][j] = Math.Pow(-1.0, (double)(i + j)) * Determinant(ExtractCoFacteur(Copy().Transpose().Matrix, i, j));
                    }
                }
            };
            Stopwatch stopwatch = new();
            stopwatch.Start();
            ParallelLoopResult result= Parallel.For(0, T, (t) => func(t, T));
            if (result.IsCompleted)
            {
                stopwatch.Stop();
                Console.WriteLine($"Parallel execution time: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
        else
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            for (int i = 0; i < mat.Shape[0]; i++)
            {
                for (int j = 0; j < mat.Shape[1]; j++)
                {
                    mat.Matrix[i][j] = Math.Pow(-1.0, (double)(i + j)) * Determinant(ExtractCoFacteur(Copy().Transpose().Matrix, i, j));
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Sequential execution time: {stopwatch.ElapsedMilliseconds} ms");
        }
        return mat;
    }

    /**
     * Fonction pour ajouter une colonne à la matrice courante
     *  array: tableau de tableau qui contient la colonne à ajouter
     *  place: end ou front pour spécifier si la colonne doit être ajoutée à la fin
     *  T: nombre de thread ou processeur logique qui sera utilisé pour ajouter la colonne à la matrice courante
     */
    public Matrice Hstack(double[] array, string place = "front",int T=1)
    {
        Matrice matrice = new([Shape[0], Shape[1] + 1]);
        if (place=="end")
        {
            if (T > 1)
            {
                Action<int, int> func = (int t, int T) =>
                {

                    for (int i = t; i < Shape[0]; i += T)
                    {
                        for (int j = 0; j < Shape[1]; j++)
                        {
                            matrice.Matrix[i][j] = Matrix[i][j];
                        }
                        matrice.Matrix[i][Shape[1]] = array[i];
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < Shape[0]; i++)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        matrice.Matrix[i][j] = Matrix[i][j];
                    }
                    matrice.Matrix[i][Shape[1]] = array[i];
                }
            }
        }
        else
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < Shape[0]; i += T)
                    {
                        matrice.Matrix[i][0] = array[i];
                        for (int j = 0; j < Shape[1]; j++)
                        {
                            matrice.Matrix[i][j + 1] = Matrix[i][j];
                        }
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < Shape[0]; i++)
                {
                    matrice.Matrix[i][0] = array[i];
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        matrice.Matrix[i][j + 1] = Matrix[i][j];
                    }
                }
            }
        }
        return matrice;
    }

    /**
     * Fonction pour ajouter plusieurs colonnes à la matrice courante
     *  array: tableau de tableau qui contient les colonnes à ajouter
     *  place: end ou front pour spécifier si les colonnes doivent être ajoutées à la fin ou au début de la matrice
     *  T: nombre de thread ou processeur logique qui sera utilisé pour ajouter les colonnes à la matrice courante
     */
    public Matrice Hstack(double[][] array, string place = "front",int T=1)
    {
        Matrice mat = new(array);
        Matrice matrice = new([Shape[0], Shape[1] + mat.Shape[0]]);
        mat = mat.Transpose();

        if (place=="end")
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < Shape[0]; i+=T)
                    {
                        for (int j = 0; j < Shape[1]; j++)
                        {
                            matrice.Matrix[i][j] = Matrix[i][j];
                        }
                    }
                    for (int i = t; i < matrice.Shape[0]; i+=T)
                    {
                        for (int j = Shape[1]; j < matrice.Shape[1]; j++)
                        {
                            matrice.Matrix[i][j] = mat.Matrix[i][j - Shape[1]];
                        }
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < Shape[0]; i++)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        matrice.Matrix[i][j] = Matrix[i][j];
                    }
                }
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    for (int j = Shape[1]; j < matrice.Shape[1]; j++)
                    {
                        matrice.Matrix[i][j] = mat.Matrix[i][j - Shape[1]];
                    }
                }
            }
        }
        else
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < matrice.Shape[0]; i+=T)
                    {
                        for (int j = 0; j < mat.Shape[1]; j++)
                        {
                            matrice.Matrix[i][j] = mat.Matrix[i][j];
                        }
                    }
                    for (int i = t; i < matrice.Shape[0]; i+=T)
                    {
                        for (int j = 0; j < Shape[1]; j++)
                        {
                            matrice.Matrix[i][j + mat.Shape[1]] = Matrix[i][j];
                        }
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    for (int j = 0; j < mat.Shape[1]; j++)
                    {
                        matrice.Matrix[i][j] = mat.Matrix[i][j];
                    }
                }
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        matrice.Matrix[i][j + mat.Shape[1]] = Matrix[i][j];
                    }
                }
            }
        }
        return matrice;
    }

    /**
     * Fonction pour ajouter une ligne à la matrice courante
     *  array: tableau de tableau qui contient la ligne à ajouter
     *  place: end ou front pour spécifier si la ligne doit être ajoutée à la fin ou au début de la matrice
     *  T: nombre de thread ou processeur logique qui sera utilisé pour ajouter les lignes à la matrice courante
     */
    public Matrice Vstack(double[] array,string place="front",int T=1)
    {
        Matrice matrice = new([Shape[0] + 1, Shape[1]]);
        if (place=="end")
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < matrice.Shape[0]; i += T)
                    {
                        if (i == matrice.Shape[0] - 1)
                        {
                            matrice.Matrix[i] = array;
                        }
                        else
                        {
                            matrice.Matrix[i] = Matrix[i];
                        }
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (i == matrice.Shape[0] - 1)
                    {
                        matrice.Matrix[i] = array;
                    }
                    else
                    {
                        matrice.Matrix[i] = Matrix[i];
                    }
                }
            }
        }
        else
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < matrice.Shape[0]; i += T)
                    {
                        if (i == 0)
                        {
                            matrice.Matrix[i] = array;
                        }
                        else
                        {
                            matrice.Matrix[i] = Matrix[i - 1];
                        }
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (i == 0)
                    {
                        matrice.Matrix[i] = array;
                    }
                    else
                    {
                        matrice.Matrix[i] = Matrix[i - 1];
                    }
                }
            }
        }
        return matrice;
    }


    /**
     * Fonction pour ajouter plusieurs lignes à la matrice courante
     *  array: tableau de tableau qui contient les lignes à ajouter
     *  place: end ou front pour spécifier si les lignes doivent être ajoutées à la fin ou au début de la matrice
     *  T: nombre de thread ou processeur logique qui sera utilisé pour ajouter les lignes à la matrice courante
     */
    public Matrice Vstack(double[][] array, string place = "front",int T=1)
    {
        Matrice matrice = new([Shape[0] + array.Length, Shape[1]]);
        if (place=="end")
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < Shape[0]; i += T)
                    {
                        matrice.Matrix[i] = Matrix[i];
                    }
                    for (int i = t; i < matrice.Shape[0]; i += T)
                    {
                        matrice.Matrix[i] = array[i - Shape[0]];
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < Shape[0]; i++)
                {
                    matrice.Matrix[i] = Matrix[i];
                }
                for (int i = Shape[0]; i < matrice.Shape[0]; i++)
                {
                    matrice.Matrix[i] = array[i - Shape[0]];
                }
            }
        }
        else
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < matrice.Shape[0]; i += T)
                    {
                        matrice.Matrix[i] = array[i];
                    }
                    for (int i = t; i < matrice.Shape[0]; i += T)
                    {
                        matrice.Matrix[i + array.Length] = Matrix[i];
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < array.Length; i++)
                {
                    matrice.Matrix[i] = array[i];
                }
                for (int i = array.Length; i < matrice.Shape[0]; i++)
                {
                    matrice.Matrix[i] = Matrix[i - array.Length];
                }
            }
        }
        return matrice;
    }

    /**
     *  fonction pour extraire la matrice de cofacteur liée au coefficient (i,j) de la matrice courante 
     */
    public static double[][] ExtractCoFacteur(double[][] matrice, int i, int j)
    {
        int m = matrice.Length - 1;
        double[][] result = new double[m][];

        int k = 0;
        int t = 0;

        while (k < matrice.Length)
        {
            if (k != i)
            {
                double[] tmp = new double[m];
                int z = 0;
                int e = 0;
                while (e < matrice[k].Length)
                {
                    if (e != j)
                    {
                        tmp[z++] = matrice[k][e];
                    }
                    e++;
                }
                result[t++] = tmp;
            }
            k++;
        }
        return result;
    }

    /**
     *  fonction pour extraire une sous matrice
     */
    private static double[][] ExtractCoMatrix(double[][] matrice,int j)
    {

        int m = matrice.Length-1;
        double[][] result = new double[m][];

        int k = 1;
        int t = 0;

        while (k < matrice.Length)
        {
            double[] tmp = new double[m];
            int z = 0;
            int e = 0;
            while (e < matrice[k].Length)
            {
                if (e != j)
                {
                    tmp[z++]=matrice[k][e];
                }
                e++;
            }
            result[t++]=tmp;
            k++;
        }
        return result;
    }

    /**
     *  fonction pour calculer la somme de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la somme ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la somme de la matrice courante
     */
    public double Sum(int T=1)
    {
        double som = 0.0;
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < Shape[0]; i += T)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        som += Matrix[i][j];
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < Shape[0]; i ++)
            {
                for (int j = 0; j < Shape[1]; j++)
                {
                    som += Matrix[i][j];
                }
            }
        }
        return som;
    }

    /**
     *  fonction pour calculer la somme cumulée de chaque ligne de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la somme cumulée ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la somme cumulée de la matrice courante
     */
    public double[] CumSum(int T=1)
    {
        double[] som = new double[Shape[0]];
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < Shape[0]; i += T)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        som[i] += Matrix[i][j];
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                for (int j = 0; j < Shape[1]; j++)
                {
                    som[i] += Matrix[i][j];
                }
            }
        }
        return som;
    }

    /**
     *  fonction pour calculer la moyenne de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la moyenne ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la moyenne de la matrice courante
     */
    public double Mean(int T=1)
    {
        return Sum(T) / (Shape[0] * Shape[1]);
    }

    /**
     *  fonction pour calculer la moyenne cumulée de chaque ligne de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la moyenne cumulée ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la moyenne cumulée de la matrice courante
     */
    public double[] CumMean(int T=1)
    {
        double[] arrayMean = new double[Shape[0]];
        double[] arraySum = CumSum(T);
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < Shape[0]; i+=T)
                {
                    arrayMean[i] = arraySum[i] / Shape[1];
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                arrayMean[i] = arraySum[i] / Shape[1];
            }
        }
        return arrayMean;
    }

    /**
     *  fonction pour calculer la variance de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la variance ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la variance de la matrice courante
     */
    public double Var(int T=1)
    {
        double mean = Mean(T);
        double cumsum = 0.0;
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < Shape[0]; i+=T)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        cumsum += Math.Pow(Matrix[i][j] - mean, 2.0);
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                for (int j = 0; j < Shape[1]; j++)
                {
                    cumsum += Math.Pow(Matrix[i][j] - mean, 2.0);
                }
            }
        }
        return cumsum / (Shape[0] * Shape[1]);
    }

    /**
     *  fonction pour calculer l'écart type de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de l'écart type ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer l'écart type de la matrice courante
     */
    public double Sd(int T=1)
    {
        return Math.Sqrt(Var(T));
    }

    /**
     *  fonction pour calculer la variance cumulée de chaque ligne de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la variance cumulée ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la variance cumulée de la matrice courante
     */
    public double[] CumVar(int T=1)
    {
        double[] arrayVar = new double[Shape[0]];
        double[] arrayMean = CumMean(T);
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < Shape[0]; i += T)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        arrayVar[i] += (Math.Pow(Matrix[i][j] - arrayMean[i], 2.0) / Shape[1]);
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                for (int j = 0; j < Shape[1]; j++)
                {
                    arrayVar[i] += (Math.Pow(Matrix[i][j] - arrayMean[i], 2.0) / Shape[1]);
                }
            }
        }
        return arrayVar;
    }

    /**
     *  fonction pour calculer l'écart type cumulé de chaque ligne de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de l'écart type cumulé ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer l'écart type cumulé de la matrice courante
     */
    public double[] CumSd(int T=1)
    {
        double[] cumVar = CumVar(T);
        double[] cumsd = new double[Shape[0]];
        if (T>1)
        {
            Action<int,int> func = (int t, int T) =>
            {
                for (int i = t; i < Shape[0]; i += T)
                {
                    cumsd[i] = Math.Sqrt(cumVar[i]);
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < cumVar.Length; i++)
            {
                cumsd[i] = Math.Sqrt(cumVar[i]);
            }
        }
        return cumsd;
    }

    /**
     *  fonction pour calculer la matrice inverse de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la matrice inverse ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la matrice inverse de la matrice courante
     */
    public Matrice Inverse(int T=1)
    {
        double determinant = Determinant(Matrix);
        if (determinant==0)
        {
            throw new Exception("non-invertible matrix because its determinant is zero");
        }
        else
        {
            Matrice matrice = Adjacence(T);
            matrice /= determinant;
            return matrice;
        }
    }

    /**
     *  fonction pour générer une matrice de taille shape*shape avec des valeurs 1
     *  Exemple: Matrice.Ones(3) => [[1,1,1],[1,1,1],[1,1,1]]
     *  Le paramètre T permet de paralléliser la génération de la matrice ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour générer la matrice 
     */
    public static Matrice Ones(int shape,int T=1)
    {
        Matrice matrice = new([shape,shape]);
        if (T > 1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < matrice.Shape[0]; i += T)
                {
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        matrice.Matrix[i][j] = 1.0;
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    matrice.Matrix[i][j] = 1.0;
                }
            }
        }
        return matrice;
    }

    /**
     *  fonction pour générer une matrice de taille shape*shape avec des valeurs 0
     *  Exemple: Matrice.Zeros(3) => [[0,0,0],[0,0,0],[0,0,0]]
     *  Le paramètre T permet de paralléliser la génération de la matrice ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour générer la matrice 
     */
    public static Matrice Zeros(int shape,int T=1)
    {
        Matrice matrice = new([shape,shape]);
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < matrice.Shape[0]; i += T)
                {
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        matrice.Matrix[i][j] = 0.0;
                    }
                }
            };
            Parallel.For(0, T, (t) => func(t, T));
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    matrice.Matrix[i][j] = 0.0;
                }
            }
        }
        return matrice;
    }


    /**  
     * fonction pour résoudre un système d'équation linéaire
     * en résolvant l'équation matricielle Ax = b => x = A^-1 * b
     */
    public double[] Solve(double[] beta,int T=1) => Inverse(T) * beta;


    public Matrice(double[] array)
    {
        Shape = [array.Length, 1];
        Matrix = new double[Shape[0]][];
        for (var i = 0; i < Shape[0]; i++)
        {
            Matrix[i] = new double[1];
            Matrix[i][0] = array[i];
        }
    }

    public Matrice(List<double> array)
    {
        Shape = [array.Count, 1];
        Matrix = new double[Shape[0]][];
        for (var i = 0; i < Shape[0]; i++)
        {
            Matrix[i] = new double[1];
            Matrix[i][0] = array[i];
        }
    }

    public double[] Flatten(int T = 1)
    {
        List<double> result =[];
        result = [..Matrix[0]];
        if (T > 1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < Shape[0]; i+=T)
                {
                    result.AddRange(Matrix[i]);
                }
            };
            Parallel.For(0,T,(t)=>func(t,T));
        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                result.AddRange(Matrix[i]);
            }
        }
        return [..result];
    }

    public double[] Squeeze(int T = 1)
    {
        List<double> result =[];
        if (Shape[1]==1)
        {
            if (T>1)
            {
                Action<int, int> func = (int t, int T) =>
                {
                    for (int i = t; i < Shape[0]; i+=T)
                    {
                        result.AddRange(Matrix[i]);
                    }
                };
                Parallel.For(0, T, (t) => func(t, T));
            }
            else
            {
                for (int i = 0; i < Shape[0]; i+=T)
                {
                    result.AddRange(Matrix[i]);
                }
            }
            return [..result];
        }
        throw new Exception("It is not possible to compress a matrix that has a column and row count greater than 1.");
    }

    public int[] ArgWhere(bool[] index,int T=1)
    {
        Mutex mutex = new Mutex();
        List<int> result = [];
        if (T>1)
        {
            Action<int, int> func = (int t, int T) =>
            {
                for (int i = t; i < index.Length; i+=T)
                {
                    if (index[i])
                    {
                        mutex.WaitOne();
                        result.Add(i);
                        mutex.ReleaseMutex();
                    }
                }
            };
            Parallel.For(0,T,(t)=>func(t,T));
        }
        else
        {
            for (int i = 0; i < index.Length; i++)
            {
                if(index[i])
                {
                    result.Add(i);
                }
            }
        }
        return [..result];
    }

    public static bool[] operator >(Matrice matrice,double val)
    {
        List<bool> result = [];
        for (int i = 0; i < matrice.Shape[0]; i++)
        {
            for (int j = 0; j < matrice.Shape[1]; j++)
            {
                result.Add(matrice.Matrix[i][j] > val);
            }
        }
        return [.. result];
    }

    public static bool[] operator <(Matrice matrice,double val)
    {
        List<bool> result = [];
        for (int i = 0; i < matrice.Shape[0]; i++)
        {
            for (int j = 0; j < matrice.Shape[1]; j++)
            {
                result.Add(matrice.Matrix[i][j] < val);
            }
        }
        return [.. result];
    }
    
    public static bool[] operator >=(Matrice matrice,double val)
    {
        List<bool> result = [];
        for (int i = 0; i < matrice.Shape[0]; i++)
        {
            for (int j = 0; j < matrice.Shape[1]; j++)
            {
                result.Add(matrice.Matrix[i][j] >= val);
            }
        }
        return [.. result];
    }

    public static bool[] operator <=(Matrice matrice,double val)
    {
        List<bool> result = [];
        for (int i = 0; i < matrice.Shape[0]; i++)
        {
            for (int j = 0; j < matrice.Shape[1]; j++)
            {
                result.Add(matrice.Matrix[i][j] <= val);
            }
        }
        return [.. result];
    }
    
    public static bool[] operator ==(Matrice matrice,double val)
    {
        List<bool> result = [];
        for (int i = 0; i < matrice.Shape[0]; i++)
        {
            for (int j = 0; j < matrice.Shape[1]; j++)
            {
                result.Add(matrice.Matrix[i][j].Equals(val));
            }
        }
        return [.. result];
    }
    
    public static bool[] operator !=(Matrice matrice,double val)
    {
        List<bool> result = [];
        for (int i = 0; i < matrice.Shape[0]; i++)
        {
            for (int j = 0; j < matrice.Shape[1]; j++)
            {
                result.Add(!matrice.Matrix[i][j].Equals(val));
            }
        }
        return [.. result];
    }
}