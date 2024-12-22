namespace MatriceLib;

public class Matrice
{
    private int[] 
        Shape { get;}
    private double[][] Matrix { get;}


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
     *  fonction pour afficher les coéfficients de la matricielle sous forme de chaine de caractère
     *
     */
    public override string ToString()
    {
        string result = "\n";
        for (var i = 0; i < Shape[0]; i++)
        {
            for (var j = 0; j < Shape[1]; j++)
            {
                result += $"\t{Matrix[i][j]}";
            }
            result += "\n";
        }
        return $"{result}";
    }

    // public int[] Shape()
    // {
    //     return 
    //         Shape;
    // }

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
     *  fonction pour vérifier si deux matrices ont aumoins un coéfficient différent
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
     * Fonction pour faire le produit matricielle
     */
    public  Matrice Dot(Matrice matrice1)
    {
        
        if (matrice1.Shape[1]!=this.Shape[0])
        {
            throw new Exception("colonne number of matrice1 should be the same than row number of current matrice ");
        }
        
        double[][] tmp = new double[this.Shape[0]][];
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
                    som+=matrice1.Matrix[i][k]*this.Matrix[k][j];
                }
                tmp[i][j] = som;
            }
        }
        return new Matrice(tmp);
    }
    
    

    /**
     * Fonction pour calculer la transposée d'une matrice
     */
    public Matrice Transpose()
    {
        
        double[][] matrix = new double[Shape[1]][];
        for (var i = 0; i < Shape[1]; i++)
        {
            matrix[i] = new double[Shape[0]];
            for (var j = 0; j < matrix[i].Length; j++)
            {
                
                matrix[i][j] = Matrix[j][i];
                
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
            Matrix[i] = matrix[i].ToArray();
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

    // public double Determinant()
    // {
    //     if ((Shape[0], Shape[1]) == (2, 2))
    //     {
    //         return Matrix[0][0]*Matrix[1][1]-Matrix[0][1]*Matrix[1][0];
    //     }
    //     return 0.0;
    // }

}