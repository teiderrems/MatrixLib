namespace MatriceLib;

class Input(int t,int T){
    public int t=t;
    public int T=T;
}

public class Matrice
{
    private int[] Shape { 
            get{
                return [Matrix.Length,Matrix.Length>0?Matrix[0].Length:0];
            }
        }
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
    public Matrice this[params int[] index]
    {
        get{
            double[][] result=new double[index.Length][];
            if (index.Length>1000)
            {
                int T=Environment.ProcessorCount-2;
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < index.Length; i += input.T)
                    {
                        if (index[i] < Shape[0]){

                            if (m.WaitOne())
                            {
                                result[i] = Matrix[index[i]];
                                m.ReleaseMutex();
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"{index[i]} out of range");
                        }

                    }
                }
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return new Matrice(result);
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
                return new Matrice(result);
            }
        }
    }


    public Matrice this[params bool[][] bools]{
        get{
            List<List<double>> tmp=[];
            if (Shape[0]>=100)
            {
                int T=Environment.ProcessorCount-2;
                Mutex m=new();
                int e=0;
                void F(object? obj){

                    Input input=(Input)obj!;
                    for (int i = input.t; i < Shape[0]; i+=input.T)
                    {
                        List<double> t=[];
                        int k=0;
                        for (int j = 0; j < Shape[1]; j++)
                        {
                            if (bools[i][j])
                            {
                                t.Insert(k,Matrix[i][j]);
                            }
                        }
                        if (m.WaitOne())
                        {
                            tmp.Insert(e++,t);
                            m.ReleaseMutex();
                        }
                    }
                }

                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return new(tmp);
            }
            else
            {
                for (int i = 0; i < Shape[0]; i++)
                {
                    List<double> t=[];
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        if (bools[i][j])
                        {
                            t.Add(Matrix[i][j]);
                        }
                    }
                    tmp.Add(t);
                }
                return new(tmp);
            }
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
        if (obj is not Matrice)
        {
            return false;
        }
        return this==(Matrice)obj;
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
        
        if (matrice1.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice1.
                Shape[0]; i+=input.T)
                {
                    if (m.WaitOne())
                    {
                        tmp[i] = new double[matrice1.Shape[1]];
                        m.ReleaseMutex();
                    }
                    for (var j = 0; j < matrice1.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]=matrice2.Matrix[i][j]+matrice1.Matrix[i][j];
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
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
        double[][] tmp = new double[matrice2.Shape[0]][];
        if (matrice1.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice1.Shape[0]; i+=input.T)
                {
                    if (m.WaitOne())
                    {
                        tmp[i] = new double[matrice1.Shape[1]];
                        m.ReleaseMutex();
                    }
                    for (var j = 0; j < matrice1.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]=matrice2.Matrix[i][j]-matrice1.Matrix[i][j];
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
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
    }


    /**
    *  opérateur pour avoir l'opposé des coefficients d'une matricielle
    *
    */
    public static Matrice operator -(Matrice matrice)
    {
         
        double[][] tmp = new double[matrice.Shape[0]][];

        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice.Shape[0]; i+=input.T)
                {
                    tmp[i] = new double[matrice.Shape[1]];
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j] = (-1)*matrice.Matrix[i][j];
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
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
        double[][] tmp = new double[matrice2.Shape[0]][];
        
        if (matrice1.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice1.Shape[0]; i+=input.T)
                {
                    if (m.WaitOne())
                    {
                        tmp[i] = new double[matrice1.Shape[1]];
                        m.ReleaseMutex();
                    }
                    for (var j = 0; j < matrice1.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]=matrice2.Matrix[i][j]*matrice1.Matrix[i][j];
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
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
        double[][] tmp = new double[matrice2.Shape[0]][];
        
        if (matrice1.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice1.
                Shape[0]; i+=input.T)
                {
                    tmp[i] = new double[matrice1.
                            Shape[1]];
                    for (var j = 0; j < matrice1.
                            Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]=matrice2.Matrix[i][j]/matrice1.Matrix[i][j];
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
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
    }

    /**
     *  opérateur pour facilité la multiplication des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator*(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice.
                Shape[0]; i+=input.T)
                {
                    tmp[i] = new double[matrice.Shape[1]];
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]=matrice.Matrix[i][j]*cte;
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
            for (var i = 0; i < matrice.
                Shape[0]; i++)
            {
                tmp[i] = new double[matrice.Shape[1]];
                for (var j = 0; j < matrice.Shape[1]; j++)
                {
                    tmp[i][j]=matrice.Matrix[i][j]*cte;
                }
            }
            return new Matrice(tmp);
        }
    }

    /**
     *  Fonction pour récupérer les coefficients uniques contenu dans un tableau
     *
     */
    public static double[] Unique(double[] array)
    {
        List<double> list =[];
        if (array.Length>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (int i = input.t; i < array.Length; i+=input.T)
                {
                    if (m.WaitOne())
                    {
                        if (!list.Contains(array[i]))
                        {
                            list.Add(array[i]);
                        }
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return [..list];
        }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (!list.Contains(array[i]))
                {
                    list.Add(array[i]);
                }
            }
            return [..list];
        }
    }

    /**
     *  Fonction pour générer la matrice identité
     *  Exemple: Matrice mat=Matrice.Identity(3); 
     *  Console.WriteLine(mat);=>[[1,0,0],[0,1,0],[0,0,1]]
     */
    public static Matrice Identity(int shape,int T=1)
    {  
        Matrice matrice = new([shape,shape]);
        if (T>1 || shape>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < matrice.Shape[0]; i += input.T)
                {
                    if (m.WaitOne())
                    {
                        matrice.Matrix[i][i]=1.0;
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return matrice;
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                matrice.Matrix[i][i] = 1.0;
            }
            return matrice;
        }
        
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

            if(matrice.Shape[0]>=1000){

                Mutex m=new();
                void F(object? obj){
                    Input input=(Input)obj!;
                    for (int i = input.t; i < matrice.Shape[0]; i+=input.T)
                    {
                        double som = 0.0;
                        for (int j = 0; j < matrice.Shape[1]; j++)
                        {
                            som += matrice.Matrix[i][j] * array[j];
                        }
                        if (m.WaitOne())
                        {
                            result[i] = som;
                            m.ReleaseMutex();
                        }
                    }
                }
                int T=Environment.ProcessorCount-2;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return result;
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
                return result;
            }
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
        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice.Shape[0]; i+=input.T)
                {
                    tmp[i] = new double[matrice.Shape[1]];
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]+=cte;
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
            for (var i = 0; i < matrice.Shape[0]; i++)
            {
                for (var j = 0; j < matrice.Shape[1]; j++)
                {
                    tmp[i][j]+=cte;
                }
            }

            return new Matrice(tmp);
        }
    }
    
    /**
     *  opérateur pour facilité l'addition des coéfficients d'une matrice avec une constante
     *  
     */
    public static Matrice operator+(double cte,Matrice matrice)
    {
        return matrice+cte;
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator-(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice.Shape[0]; i+=input.T)
                {
                    tmp[i] = new double[matrice.Shape[1]];
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]-=cte;
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
            for (var i = 0; i < matrice.Shape[0]; i++)
            {
                for (var j = 0; j < matrice.Shape[1]; j++)
                {
                    tmp[i][j]-=cte;
                }
            }

            return new Matrice(tmp);
        }
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator-(double cte,Matrice matrice)
    {
        double[][] tmp = matrice.Matrix;
        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice.Shape[0]; i+=input.T)
                {
                    tmp[i] = new double[matrice.Shape[1]];
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]=cte-tmp[i][j];
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
            for (var i = 0; i < matrice.Shape[0]; i++)
            {
                for (var j = 0; j < matrice.Shape[1]; j++)
                {
                    tmp[i][j]=cte-tmp[i][j];
                }
            }

            return new Matrice(tmp);
        }
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator/(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){
                Input input=(Input)obj!;
                for (var i = input.t; i < matrice.Shape[0]; i+=input.T)
                {
                    tmp[i] = new double[matrice.Shape[1]];
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            tmp[i][j]/=cte;
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
            for (var i = 0; i < matrice.Shape[0]; i++)
            {
                for (var j = 0; j < matrice.Shape[1]; j++)
                {
                    tmp[i][j]/=cte;
                }
            }

            return new Matrice(tmp);
        }
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator/(double cte,Matrice matrice)
    {
        return matrice/cte;
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
        return !(matrice1==matrice2);
    }


    /**
     *  Fonction faire le produit matriciel
     *  matrice1: la matrice avec laquelle on veut faire le produit avec la matrice courante
     *  T: nombre de thread ou processeur logique qui sera utilisé pour calculer le produit matriciel
     */
    public Matrice Dot(Matrice matrice1,int T=1)
    {
        if (matrice1.Shape[0]!=Shape[1])
        {
            throw new Exception("colonne number of matrice1 should be the same than row number of current matrice ");
        }
        double[][] tmp = new double[Shape[0]][];
        if (T>1 || Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (var i = input.t; i < Shape[0]; i += input.T)
                {
                    if (m.WaitOne())
                    {
                        tmp[i] = new double[Shape[1]];
                        m.ReleaseMutex();
                    }
                    double som = 0;
                    for (var j = 0; j < Shape[1]; j++)
                    {
                        for (var k = 0; k < matrice1.Shape[0]; k++)
                        {
                            som += matrice1.Matrix[i][k] * Matrix[k][j];
                        }
                        if (m.WaitOne())
                        {
                            tmp[i][j] = som;
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(tmp);
        }
        else
        {
            for (var i = 0; i < matrice1.Shape[0]; i++)
            {
                tmp[i] = new double[Shape[1]];
                double som = 0;
                for (var j = 0; j <Shape[1]; j++)
                {
                    for (var k = 0; k < Shape[0]; k++)
                    {
                        som += matrice1.Matrix[i][k] * Matrix[k][j];
                    }
                    tmp[i][j] = som;
                }
            }
            return new Matrice(tmp);
        }
        
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
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < matrice.Shape[0]; i += input.T)
                    {
                        double som = 0.0;
                        for (int j = 0; j < matrice.Shape[1]; j++)
                        {
                            som += matrice.Matrix[i][j] * array[j];
                        }
                        if (m.WaitOne())
                        {
                            result[i] = som;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return result;
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
                return result;
            }
            
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
        if (T>1 || Shape[1]>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (var i = input.t; i < Shape[1]; i += input.T)
                {
                    double[] tmp=new double[Shape[0]];
                    for (var j = 0; j < Shape[0]; j++)
                    {

                        tmp[j] = Matrix[j][i];

                    }
                    if (m.WaitOne())
                    {
                        matrix[i] = tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new Matrice(matrix);
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
            return new Matrice(matrix);
        }
        
    }

    public Matrice(double[][] matrix)
    {
        Matrix = matrix;
    }
    
    public Matrice(List<List<double>> matrix)
    {
        Matrix = new double[matrix.Count][];
        for (var i = 0; i < matrix.Count; i++)
        {
            Matrix[i] = [..matrix[i]];
        }
    }

    public Matrice(int[] shape)
    {
        Matrix = new double[shape[0]][];
        for (var i = 0; i <shape[0]; i++)
        {
            Matrix[i] = new double[shape[1]];
        }
    }

    public Matrice(double[] array,int[] shape)
    {
        Matrix=new double[shape[0]][];
        var k = 0;
        for (var i = 0; i <shape[0]; i++)
        {
            Matrix[i] = new double[shape[0]];
            for (var j = 0; j <shape[1]; j++)
            {
                Matrix[i][j] = array[k];
                k++;
            }
        }
    }

    public Matrice(List<double> array,int[] shape)
    {
        Matrix=new double[shape[0]][];
        var k = 0;
        for (var i = 0; i <shape[0]; i++)
        {
            Matrix[i] = new double[shape[0]];
            for (var j = 0; j <shape[1]; j++)
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
            if (matrice[0].Length>=100)
            {
                Mutex m=new();
                void F(object? obj){

                    Input input=(Input)obj!;
                    for (int i = j+input.t; i < matrice[0].Length; i+=input.T)
                    {
                        double tmp=Math.Pow(-1.0, (double)i) * matrice[0][i] * Determinant(ExtractCoMatrix(matrice, i), i);
                        if (m.WaitOne())
                        {
                            deter +=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                int T=Environment.ProcessorCount-2;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return deter;
            }
            else
            {
                for (int i = j; i < matrice[0].Length; i++)
                {
                    deter += Math.Pow(-1.0, (double)i) * matrice[0][i] * Determinant(ExtractCoMatrix(matrice, i), i);
                }
                return deter;
            }
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
        if (T>1 || Shape[0]>=10)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < mat.Shape[0]; i += input.T)
                {
                    for (int j = 0; j < mat.Shape[1]; j++)
                    {
                        double tmp=Math.Pow(-1.0, (double)(i + j)) * Determinant(ExtractCoFacteur(Copy().Transpose().Matrix, i, j));
                        if (m.WaitOne())
                        {
                            mat.Matrix[i][j] =tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return mat;
        }
        else
        {
            for (int i = 0; i < mat.Shape[0]; i++)
            {
                for (int j = 0; j < mat.Shape[1]; j++)
                {
                    mat.Matrix[i][j] = Math.Pow(-1.0, (double)(i + j)) * Determinant(ExtractCoFacteur(Copy().Transpose().Matrix, i, j));
                }
            }
            return mat;
        }
        
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
            if (T > 1 || Shape[0]>=1000)
            {
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;

                    for (int i = input.t; i < Shape[0]; i += input.T)
                    {
                        double[] tmp=new double[matrice.Shape[1]];
                        for (int j = 0; j < Shape[1]; j++)
                        {
                            tmp[j] = Matrix[i][j];
                        }
                        tmp[Shape[1]]=array[i];
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
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
            if (T > 1 || Shape[0]>=1000)
            {
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;

                    for (int i = input.t; i < Shape[0]; i += input.T)
                    {
                        double[] tmp=new double[matrice.Shape[1]];
                        tmp[0]=array[i];
                        for (int j = 0; j < Shape[1]; j++)
                        {
                            tmp[j+1] = Matrix[i][j];
                        }
                        
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
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
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < matrice.Shape[0]; i += input.T)
                    {
                        double[] tmp=new double[matrice.Shape[1]];
                        for (int j = 0; j < matrice.Shape[1]; j++)
                        {
                            if (j<Shape[1])
                            {
                                tmp[j]=Matrix[i][j];
                            }
                            else
                            {
                                
                                tmp[j]=mat.Matrix[i][j-Shape[1]];
                            }
                        }
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return matrice;
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
                return matrice;
            }
        }
        else
        {
            if (T>1)
            {
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < matrice.Shape[0]; i += input.T)
                    {
                        double[] tmp=new double[matrice.Shape[1]];
                        for (int j = 0; j < matrice.Shape[1]; j++)
                        {
                            if (j<mat.Shape[1])
                            {
                                tmp[j]=mat.Matrix[i][j];
                            }
                            else
                            {
                                
                                tmp[j]=Matrix[i][j-mat.Shape[1]];
                            }
                        }
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return matrice;
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
                return matrice;
            }
        }
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
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < matrice.Shape[0]; i += input.T)
                    {
                        double[] tmp=[];
                        if (i == matrice.Shape[0] - 1)
                        {
                            tmp= array;
                        }
                        else
                        {
                            tmp= Matrix[i];
                        }
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
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
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < matrice.Shape[0]; i += input.T)
                    {
                        double[] tmp=[];
                        if (i == 0)
                        {
                            tmp= array;
                        }
                        else
                        {
                            tmp= Matrix[i-1];
                        }
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
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
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < Shape[0]; i += input.T)
                    {
                        double[] tmp= Matrix[i];
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                    input.t=0;
                    for (int i = input.t+Shape[0]; i < matrice.Shape[0]; i += input.T)
                    {
                        double[] tmp= array[i- Shape[0]];
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
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
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < array.Length; i += input.T)
                    {
                        double[] tmp= array[i];
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                    input.t=0;
                    for (int i = input.t; i < Shape[0]; i += input.T)
                    {
                        double[] tmp= Matrix[i];
                        if (m.WaitOne())
                        {
                            matrice.Matrix[i + array.Length]=tmp;
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
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
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i += input.T)
                {
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        if (m.WaitOne())
                        {
                            som += Matrix[i][j];
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return som;
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
            return som;
        }
        
    }

    /**
     *  fonction pour calculer la somme cumulée de chaque ligne de la matrice courante
     *  Le paramètre T permet de paralléliser le calcul de la somme cumulée ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour calculer la somme cumulée de la matrice courante
     */
    public double[] CumSum(int T=1)
    {
        double[] som = new double[Shape[0]];
        if (T>1 || Shape[0]>=1000)
        {
            Mutex m=new(); 
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i += input.T)
                {
                    double s=0.0;
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        s+= Matrix[i][j];
                    }
                    if (m.WaitOne())
                    {
                        som[i]=s;
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return som;
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
            return som;
        }
        
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
        if (T>1 || Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i += input.T)
                {
                    double mean= arraySum[i] / Shape[1];
                    if (m.WaitOne())
                    {
                        arrayMean[i]=mean;
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return arrayMean;
        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                arrayMean[i] = arraySum[i] / Shape[1];
            }
            return arrayMean;
        }
        
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
        if (T>1 || Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i += input.T)
                {
                    double v=0.0;
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        v+= Math.Pow(Matrix[i][j] - mean, 2.0);
                    }
                    if (m.WaitOne())
                    {
                        cumsum+=v;
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return cumsum / (Shape[0] * Shape[1]);
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
            return cumsum / (Shape[0] * Shape[1]);
        }
        
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
        if (T>1 || Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i += input.T)
                {
                    double v=0.0;
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        v += Math.Pow(Matrix[i][j] - arrayMean[i], 2.0) / Shape[1];
                    }
                    if (m.WaitOne())
                    {
                        arrayVar[i]=v;
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return arrayVar;
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
            return arrayVar;
        }
        
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
        if (T>1 || Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i += input.T)
                {
                    double sd = Math.Sqrt(cumVar[i]);
                    if (m.WaitOne())
                    {
                        cumsd[i]=sd;
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return cumsd;
        }
        else
        {
            for (int i = 0; i < cumVar.Length; i++)
            {
                cumsd[i] = Math.Sqrt(cumVar[i]);
            }
            return cumsd;
        }
        
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
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < matrice.Shape[0]; i += input.T)
                {
                    double[] tmp=new double[matrice.Shape[1]];
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        tmp[j] = 1.0;
                    }
                    if (m.WaitOne())
                    {
                        matrice.Matrix[i]=tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return matrice;
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
            return matrice;
        }
        
    }

    /**
     *  fonction pour générer une matrice de taille shape*shape avec des valeurs 0
     *  Exemple: Matrice.Zeros(3) => [[0,0,0],[0,0,0],[0,0,0]]
     *  Le paramètre T permet de paralléliser la génération de la matrice ie le nombre de thread ou processeur logique qui 
     *  sera utilisé pour générer la matrice 
     */
    public static Matrice Zeros(int shape)
    {
        return new([shape,shape]);
    }


    /**  
     * fonction pour résoudre un système d'équation linéaire
     * en résolvant l'équation matricielle Ax = b => x = A^-1 * b
     */
    public double[] Solve(double[] beta,int T=1) => Inverse(T) * beta;


    public Matrice(double[] array)
    {
        Matrix = new double[array.Length][];
        for (var i = 0; i < array.Length; i++)
        {
            Matrix[i] = new double[1];
            Matrix[i][0] = array[i];
        }
    }

    public Matrice(List<double> array)
    {
        Matrix = new double[array.Count][];
        for (var i = 0; i < array.Count; i++)
        {
            Matrix[i] = new double[1];
            Matrix[i][0] = array[i];
        }
    }

    public double[] Flatten(int T = 1)
    {
        List<double> result =[];
        result = [..Matrix[0]];
        if (T > 1 || Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i += input.T)
                {
                    if (m.WaitOne())
                    {
                        result.AddRange(Matrix[i]);
                        m.ReleaseMutex();
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return [..result];
        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                result.AddRange(Matrix[i]);
            }
            return [..result];
        }
        
    }

    public double[] Squeeze(int T = 1)
    {
        List<double> result =[];
        if (Shape[1]==1)
        {
            if (T>1 || Shape[0]>=1000)
            {
                Mutex m=new();
                void F(object? obj)
                {
                    Input input=(Input)obj!;
                    for (int i = input.t; i < Shape[0]; i += input.T)
                    {
                        if (m.WaitOne())
                        {
                            result.AddRange(Matrix[i]);
                            m.ReleaseMutex();
                        }
                    }
                }
                T=T==1?Environment.ProcessorCount-2:T;
                List<Thread> threads=[];
                for (int i = 0; i < T; i++)
                {
                    threads.Add(new Thread(start:F));
                    threads[i].Start(new Input(i,T));
                }
                threads.ForEach(t=>t.Join());
                m.Dispose();
                return [..result];
            }
            else
            {
                for (int i = 0; i < Shape[0]; i+=T)
                {
                    result.AddRange(Matrix[i]);
                }
                return [..result];
            }
            
        }
        throw new Exception("It is not possible to compress a matrix that has a column and row count greater than 1.");
    }

    public int[] ArgWhere(bool[] index,int T=1)
    {
        List<int> result = [];
        if (T>1)
        {
            Mutex m=new();
            void F(object? obj)
            {
                Input input=(Input)obj!;
                for (int i = input.t; i < index.Length; i += input.T)
                {
                    if (index[i])
                    {
                        if (m.WaitOne())
                        {
                            result.Add(i);
                            m.ReleaseMutex();
                        }
                    }
                }
            }
            T=T==1?Environment.ProcessorCount-2:T;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return [..result];
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
            return [..result];
        }
        
    }

    public static bool[][] operator >(Matrice matrice, double val)
    {
        bool[][] result = new bool[matrice.Shape[0]][];

        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (m.WaitOne())
                    {
                        result[i]=new bool[matrice.Shape[1]];
                        m.ReleaseMutex();
                    }
                    bool[] tmp=new bool[matrice.Shape[1]];
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        tmp[j]=matrice.Matrix[i][j] > val;
                    }
                    if (m.WaitOne())
                    {
                        result[i]=tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return result;
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                result[i]=new bool[matrice.Shape[1]];
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    result[i][j]=matrice.Matrix[i][j] > val;
                }
            }
            return result;
        }
    }

    public static bool[][] operator <(Matrice matrice, double val)
    {
        bool[][] result = new bool[matrice.Shape[0]][];

        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (m.WaitOne())
                    {
                        result[i]=new bool[matrice.Shape[1]];
                        m.ReleaseMutex();
                    }
                    bool[] tmp=new bool[matrice.Shape[1]];
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        tmp[j]=matrice.Matrix[i][j] < val;
                    }
                    if (m.WaitOne())
                    {
                        result[i]=tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return result;
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                result[i]=new bool[matrice.Shape[1]];
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    result[i][j]=matrice.Matrix[i][j] < val;
                }
            }
            return result;
        }
    }

    public static bool[][] operator >=(Matrice matrice,double val)
    {
        bool[][] result = new bool[matrice.Shape[0]][];

        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (m.WaitOne())
                    {
                        result[i]=new bool[matrice.Shape[1]];
                        m.ReleaseMutex();
                    }
                    bool[] tmp=new bool[matrice.Shape[1]];
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        tmp[j]=matrice.Matrix[i][j] >= val;
                    }
                    if (m.WaitOne())
                    {
                        result[i]=tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return result;
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                result[i]=new bool[matrice.Shape[1]];
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    result[i][j]=matrice.Matrix[i][j] >= val;
                }
            }
            return result;
        }
    }

    public static bool[][] operator <=(Matrice matrice,double val)
    {
        bool[][] result = new bool[matrice.Shape[0]][];

        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (m.WaitOne())
                    {
                        result[i]=new bool[matrice.Shape[1]];
                        m.ReleaseMutex();
                    }
                    bool[] tmp=new bool[matrice.Shape[1]];
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        tmp[j]=matrice.Matrix[i][j] <= val;
                    }
                    if (m.WaitOne())
                    {
                        result[i]=tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return result;
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                result[i]=new bool[matrice.Shape[1]];
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    result[i][j]=matrice.Matrix[i][j]<= val;
                }
            }
            return result;
        }
    }
    
    public static bool[][] operator ==(Matrice matrice,double val)
    {
        bool[][] result = new bool[matrice.Shape[0]][];

        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (m.WaitOne())
                    {
                        result[i]=new bool[matrice.Shape[1]];
                        m.ReleaseMutex();
                    }
                    bool[] tmp=new bool[matrice.Shape[1]];
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        tmp[j]=matrice.Matrix[i][j] == val;
                    }
                    if (m.WaitOne())
                    {
                        result[i]=tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return result;
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                result[i]=new bool[matrice.Shape[1]];
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    result[i][j]=matrice.Matrix[i][j] == val;
                }
            }
            return result;
        }
    }
    
    public static bool[][] operator !=(Matrice matrice,double val)
    {
        bool[][] result = new bool[matrice.Shape[0]][];

        if (matrice.Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (m.WaitOne())
                    {
                        result[i]=new bool[matrice.Shape[1]];
                        m.ReleaseMutex();
                    }
                    bool[] tmp=new bool[matrice.Shape[1]];
                    for (int j = 0; j < matrice.Shape[1]; j++)
                    {
                        tmp[j]=matrice.Matrix[i][j] != val;
                    }
                    if (m.WaitOne())
                    {
                        result[i]=tmp;
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return result;
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                result[i]=new bool[matrice.Shape[1]];
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    result[i][j]=matrice.Matrix[i][j] != val;
                }
            }
            return result;
        }
    }


    public static Matrice Random(int n){

        double[][] tmp=new double[n][];
        Random random=new();
        if (n>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                Input input=(Input)obj!;
                for (int i = input.t; i < n; i+=input.T)
                {
                    List<double> res=[]; 
                    for (int j = 0; j < n; j++)
                    {
                        res.Add(random.NextDouble());
                    }
                    if(m.WaitOne()){
                        tmp[i]=[..res];
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new(tmp);
        }
        else
        {
            for (int i = 0; i < n; i++)
            {
                tmp[i]=new double[n]; 
                for (int j = 0; j < n; j++)
                {
                    tmp[i][j]=random.NextDouble();
                }
            }
            return new(tmp);
        }
    }


    public static Matrice Random(double min,double max,int n){

        double[][] tmp=new double[n][];
        Random random=new();

        if (n>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                Input input=(Input)obj!;
                for (int i = input.t; i < n; i+=input.T)
                {
                    List<double> res=[]; 
                    for (int j = 0; j < n; j++)
                    {
                        res.Add(random.NextDouble()*(max-min)+min);
                    }
                    if(m.WaitOne()){
                        tmp[i]=[..res];
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new(tmp);
        }
        else
        {
            for (int i = 0; i < n; i++)
            {
                tmp[i]=new double[n]; 
                for (int j = 0; j < n; j++)
                {
                    tmp[i][j]=random.NextDouble()*(max-min)+min;
                }
            }
            return new(tmp);
        }
    }
    

    public static Matrice Random(double max,int n){

        return Random(0,max,n);
    }

    /**
        apply function `func` at each value in current matrix
    */
    public Matrice Apply(Func<double,double> func){

        double[][] tmp=new double[Shape[0]][];

        if (Shape[0]>=1000)
        {
            Mutex m=new();
            void F(object? obj){

                Input input=(Input)obj!;
                for (int i = input.t; i < Shape[0]; i+=input.T)
                {
                    List<double> res=[]; 
                    for (int j = 0; j < Shape[1]; j++)
                    {
                        res.Add(func(Matrix[i][j]));
                    }
                    if(m.WaitOne()){
                        tmp[i]=[..res];
                        m.ReleaseMutex();
                    }
                }
            }
            int T=Environment.ProcessorCount-2;
            List<Thread> threads=[];
            for (int i = 0; i < T; i++)
            {
                threads.Add(new Thread(start:F));
                threads[i].Start(new Input(i,T));
            }
            threads.ForEach(t=>t.Join());
            m.Dispose();
            return new(tmp);

        }
        else
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                tmp[i]=new double[Shape[1]]; 
                for (int j = 0; j < Shape[1]; j++)
                {
                    tmp[i][j]=func(Matrix[i][j]);
                }
            }
            return new(tmp);
        }
    }
}