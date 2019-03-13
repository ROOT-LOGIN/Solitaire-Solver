using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Card = System.Int32;
using ll = System.Int64;

#if false
//#define rep(var,n)  for(int var=0;var<(n);var++)
//#define all(c)  (c).begin(),(c).end()
//#define tr(i, c)  for(auto i=(c).begin(); i!=(c).end(); i++)
//#define found(e, s)  ((s).find(e) != (s).end())




int char_to_card_num(char ch);
int char_to_card_suite(char ch);
int trans_str(std::vector<int>& v, const char* s);

int read_card_num(FILE* fp);
int read_card_suite(FILE* fp);

std::vector<int> read_card_nums(FILE* fp);
std::vector<Card> read_cards(FILE* fp);

class CardHelper
{
    public const int HEART = 0;
    public const int CLUB    = 1;
    public const int DIAMOND = 2;
    public const int SPADE   = 3;

    public const int VACANT = -1;
    public const char VACANT_CH = '~';
    public const string VACANT_STR = "~";

    static readonly string[] num_full = new [] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    static readonly string num_single = "A234567890JQK";
    static readonly string[] suite_full = new [] { '♡', '♣', '♢', '♠' };
    static readonly string suite_single = "hcds";

    public static bool is_black(int suite) { return suite % 2 != 0; }

    public static int make_card(int num, int suite)
    {  // 0-12, 0-3
        if (num < 0 || suite < 0) return VACANT;
        return (num << 2) | (suite);
    }

    public static int card_num(Card card) { return (card >= 0) ? (card >> 2) : -1; }
    public static int card_suite(Card card) { return (card >= 0) ? (card & 3) : -1; }

    public static bool pilable(Card card_below, Card card_above)
    {
        return (is_black(card_suite(card_below)) != is_black(card_suite(card_above))
                && card_num(card_below) == card_num(card_above) + 1);
    }

    public static bool is_valid_card(Card card)
    {
        int num = card_num(card), suite = card_suite(card);
        return ((0 <= num && num < 13) && (0 <= suite && suite < 4));
    }

    public static char _single(Card card)
    {
        if (card == VACANT) return VACANT_CH;
        // int num = card_num(card);
        // assert(0 <= num && num < 13);
        return num_single[card_num(card)];
    }
    public static string _full(Card card)
    {
        if (card == VACANT) return VACANT_STR;

        int num = card_num(card);
        int suite = card_suite(card);
        return $"{num_full[num]}{suite_full[suite]}";
    }

    public static string _full_cards(IList<Card> cards, string delim = " ")
    {
        return string.Join(delim, cards.Select(c => _full(c)));
    }

    public static char card_serialize(Card card)
    {
        if (!is_valid_card(card)) return VACANT_CH;
        int num = card_num(card), suite = card_suite(card);
        char c;
        if (suite < 2)
        {
            c = 'A' + suite * 13 + num;
        }
        else
        {
            c = 'a' + (suite - 2) * 13 + num;
        }
        // assert(0x20 <= c && c <= 0x7e);
        return c;
    }

    public static Card card_deserialize(char c)
    {
        if (c == VACANT_CH) return VACANT;

        int num = -1, suite = -1;
        if ('A' <= c && c <= 'M')
        {
            num = (c - 'A'); suite = 0;
        }
        if ('N' <= c && c <= 'Z')
        {
            num = (c - 'N'); suite = 1;
        }
        if ('a' <= c && c <= 'm')
        {
            num = (c - 'a'); suite = 2;
        }
        if ('n' <= c && c <= 'z')
        {
            num = (c - 'n'); suite = 3;
        }

        return make_card(num, suite);
    }


    public static int char_to_card_suite(char ch)
    {
        // returns 0-3 or -1
        switch (ch)
        {
            case 's': return SPADE;
            case 'c': return CLUB;
            case 'd': return DIAMOND;
            case 'h': return HEART;
            default: return -1;
        }
    }

   /*public static int trans_str(IList<int> v, string s)
   {
        v.clear();
        for (const char* p = s; *p; ++p) {
            int n = char_to_card_num(*p);
            if (n > 0) v.push_back(n - 1);
         }
        std::cout << s << " " << v << std::endl;
        return v.size();
    }*/

}





int read_card_num(FILE* fp)
{
    while (true)
    {
        int ch = fgetc(fp);
        if (ch == EOF) return EOF;
        if (ch == '\n') return -1;
        if (ch <= ' ') continue;
        int num = char_to_card_num(ch);
        if (num == -1)
        {
            ungetc(ch, fp);
            return -1;
        }
        else
        {
            return num;
        }
    }
}

int read_card_suite(FILE* fp)
{
    while (true)
    {
        int ch = fgetc(fp);
        if (ch == EOF) return EOF;
        if (ch == '\n') return -1;
        if (ch <= ' ') continue;
        int suite = char_to_card_suite(ch);
        if (suite == -1)
        {
            ungetc(ch, fp);
            return -1;
        }
        else
        {
            return suite;
        }
    }
}

Card read_card(FILE* fp)
{
    int num = -1;
    while (true)
    {
        int ch = fgetc(fp);
        if (ch == EOF || ch == '\n') return -1;
        if (ch <= ' ' || ch == '1') continue;
        num = char_to_card_num(ch);
        if (num == -1) return -1;
        break;
    }

    int suite = -1;
    while (true)
    {
        int ch = fgetc(fp);
        if (ch == EOF || ch == '\n') return -1;
        if (ch <= ' ') continue;
        suite = char_to_card_suite(ch);
        if (suite == -1) return -1;
        break;
    }

    return make_card(num, suite);
}

std::vector<int> read_card_nums(FILE* fp)
{
    std::vector<int> nums;
    while (true)
    {
        int num = read_card_num(fp);
        if (num == -1) break;
        nums.push_back(num);
    }
    return nums;
}

std::vector<Card> read_cards(FILE* fp)
{
    std::vector<Card> cards;
    while (true)
    {
        Card card = read_card(fp);
        if (card == -1) break;
        cards.push_back(card);
    }
    return cards;
}


#endif

/*char buf[3];


void usage()
{
    printf("usage: TriPeaks_solver <game-file>\n");
}*/


class CardHelper
{
    public const int HEART = 0;
    public const int CLUB = 1;
    public const int DIAMOND = 2;
    public const int SPADE = 3;

    public const int VACANT = -1;
    public const char VACANT_CH = '~';
    public const string VACANT_STR = "~";

    public static readonly string[] num_full = new[] { "10", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public static readonly string num_single = "A234567890JQK";
    public static readonly string[] suite_full = new[] { "♡", "♣", "♢", "♠" };
    public static readonly string suite_single = "hcds";

    public static int char_to_card_num(char ch)
    {
        // returns 0-12 or -1
        int n;
        switch (ch)
        {
            case 'A':
                n = 1; break;
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                n = (ch - '0'); break;
            case '0':
                n = 10; break;
            case 'J':
                n = 11; break;
            case 'Q':
                n = 12; break;
            case 'K':
                n = 13; break;
            default:
                n = 0; break;
        }
        return n - 1;
    }
}

/*
The core is Depth First Search Algorithm
*/
public class TriPeaks
{
    int[][] is_next = new int[14][];
    public bool solved { get; private set; } = false;

    Dictionary<ll, int> m_solve_visited = new Dictionary<ll, Card>();
    Dictionary<ll, ll> before = new Dictionary<ll, ll>();

    void init()
    {
        stop = new System.Diagnostics.Stopwatch();

        m_okmask = new Dictionary<Card, Card>();

        for (int i = 0; i < 13; ++i)
        {
            is_next[i] = new int[14];
            for (int j = 0; j < 13; ++j) is_next[i][j] = 0;
            is_next[i][(i + 1) % 13] = 1;
            is_next[i][(i + 12) % 13] = 1;
        }
    }

    static ll _key(int head, int st, int mask)
    {
        return ((ll)head << 33) | ((ll)st << 28) | (ll)mask; // 4bit, 5bit + 28bit = 33bit
    }

    static void render_key(ll key, ref int head, ref int st, ref int mask)
    {
        head = (int)((key >> 33) & 15);
        st = (int)((key >> 28) & 31);
        mask = (int)((key & 0xFFFFFFF));
    }

    const int FULL = (1 << 28) - 1;

    /*static readonly int[] ok_base = new int[]{
    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    0, 1, 2, 3, 4, 5, 6, 7, 8,
    10, 11, 13, 14, 16, 17,
    19, 21, 23 };*/

    Dictionary<int, int> m_okmask;

    // (((mask >> ok_base[i]) & 3) == 3)
    int okmask(int mask)
    {
        Card value;
        if (m_okmask.TryGetValue(mask, out value))
        {
            return value;
        }

        int ok = 0x3FF;

        if (((mask >> 0) & 3) == 3) ok |= (1 << 10);
        if (((mask >> 1) & 3) == 3) ok |= (1 << 11);
        if (((mask >> 2) & 3) == 3) ok |= (1 << 12);
        if (((mask >> 3) & 3) == 3) ok |= (1 << 13);
        if (((mask >> 4) & 3) == 3) ok |= (1 << 14);
        if (((mask >> 5) & 3) == 3) ok |= (1 << 15);
        if (((mask >> 6) & 3) == 3) ok |= (1 << 16);
        if (((mask >> 7) & 3) == 3) ok |= (1 << 17);
        if (((mask >> 8) & 3) == 3) ok |= (1 << 18);

        if (((mask >> 10) & 3) == 3) ok |= (1 << 19);
        if (((mask >> 11) & 3) == 3) ok |= (1 << 20);
        if (((mask >> 13) & 3) == 3) ok |= (1 << 21);
        if (((mask >> 14) & 3) == 3) ok |= (1 << 22);
        if (((mask >> 16) & 3) == 3) ok |= (1 << 23);
        if (((mask >> 17) & 3) == 3) ok |= (1 << 24);

        if (((mask >> 19) & 3) == 3) ok |= (1 << 25);
        if (((mask >> 21) & 3) == 3) ok |= (1 << 26);
        if (((mask >> 23) & 3) == 3) ok |= (1 << 27);

        m_okmask[mask] = ok;
        return ok;
    }

    static string _loc(int loc)
    {
        /* [1]:    0     1     2
         * [2]:   0 1   2 3   4 5
         * [3]:  0 1 2 3 4 5 6 7 8
         * [4]: 0 1 2 3 4 5 6 7 8 9
         */
        if (loc < 10)
        {
            return $"[4] - {(1 + loc)}";
        }
        else if (loc < 19)
        {
            return $"[3] - {(1 + loc - 10)}";
        }
        else if (loc < 25)
        {
            return $"[2] - {(1 + loc - 19)}";
        }
        else
        {
            return $"[1] - {(1 + loc - 25)}";
        }
    }

    List<int> pyramid, deck;

    ulong T = 0;

    static void printf(string s)
    {
        Console.Write(s);
    }
    static void lprintf(string s)
    {
        Console.WriteLine(s);
    }

    void output_solution()
    {
        if (!solved)
        {
            lprintf($"{T} steps, unsolvable and takes {stop.Elapsed}");
            return;
        }
        ll key = solve_key;
        lprintf($"{T} steps, solved in {stop.Elapsed}");
        List<ll> iter = new List<ll>();
        iter.Add(key);
        while (true)
        {
            if (!before.TryGetValue(key, out key)) break;
            iter.Add(key);
        }
        iter.Reverse();
        //reverse(all(iter));
        for (int i = 0; i < iter.Count - 1; ++i)
        {
            printf($"{(1 + i)}) ");
            ll key_b = iter[i], key_a = iter[i + 1];
            int head_b = 0, st_b = 0, mask_b = 0, head_a = 0, st_a = 0, mask_a = 0;
            render_key(key_b, ref head_b, ref st_b, ref mask_b);
            render_key(key_a, ref head_a, ref st_a, ref mask_a);
            //                printf("- (%c %d %07x) -> (%c %d %07x) : ",
            //                    num_single[head_b], st_b, mask_b, num_single[head_a], st_a, mask_a);
            if (st_a == st_b)
            {
                int maskdiff = mask_a - mask_b;
                int loc = 0;
                if (maskdiff != 0)
                {
                    for (; loc < 32; loc++)
                    {
                        int v = (1 << loc);
                        if (v > maskdiff || (maskdiff & v) != 0)
                            break;
                    }
                }
                //int loc = __builtin_ctz(maskdiff); count tail zero
                lprintf($"from pyramid {_loc(loc)} ({CardHelper.num_full[pyramid[loc] + 1]})");
            }
            else
            {
                lprintf($"turn deck ({CardHelper.num_full[deck[st_b] + 1]})");
            }
        }
    }

    ll solve_key = -1;
    System.Diagnostics.Stopwatch stop;
    int solve(int head = -1, int st = 1, int mask = 0)
    {
        if (solved) return -1;
        if (head == -1) head = deck[0];

        ll key = _key(head, st, mask);
        Card value;
        if (m_solve_visited.TryGetValue(key, out value))
        {
            // printf("// already visited (%d,%d)\n", st, mask);
            return value;
        }

        if (mask == FULL)
        {
            solved = true;
            solve_key = key;
            m_solve_visited[key] = 1;
            return 1;
        }
        else if (st == 24)
        {
            // printf("not solved. (%07x)\n", mask);
            m_solve_visited[key] = 0;
            return 0;
        }
        
        int ok = okmask(mask);

        /// printf("SOLVE(head=%d, st=%d, mask=%07x) : ok=%07x\n", 1+head, st, mask, ok);
        ll pky;
        bool possible = false;
        for (int i = 0; i < 28; ++i)
        {
            int m = 1 << i;
            if ((m & mask) != 0) continue; // もう引いたやつ
            if (i >= 10 && (ok & m) == 0) continue;
            // if (((mask >> ok_base[i]) & 3) < 3) continue;
            int card = pyramid[i];
            if (0 == is_next[head][card]) continue;
            // printf("  can draw %d at %d\n", 1+card, i);

            possible = true;
            // これを引く場合
            int next_mask = mask | (1 << i);
            pky = _key(card, st, next_mask);
            before[pky] = key;
            // printf("drawing %c at %d...\n", num_single[card], i);
            // putchar(num_single[card]);
            int r = solve(card, st, mask | (1 << i));
            if (r != 0) return 1;
        }
        // 引かずにただめくるだけ
        if (!possible)
        {
            /// printf("no drawable card...");
        }
        /// printf("go next...\n");
        /// putchar('\r');
        if (++T % 1000000 == 0)
        {
            lprintf($"{T}");
            /// putchar('.');
            //fflush(stdout);
        }
        pky = _key(deck[st], st + 1, mask);
        before[pky] = key;
        return solve(deck[st], st + 1, mask);
    }

    [MTAThread]
    static void Main(string[] argv)
    {
        int argc = argv.Length;
        if (argc != 1)
        {
            lprintf($"usage: {typeof(TriPeaks).Assembly.GetName().Name} <game-file>");
            return;
        }

        List<int> tmp = new List<int>(52);
        using (var fp = File.OpenText(argv[0]))
        {
            if(fp.Peek() == -1)
            {
                lprintf("FILE IS EMPTY!");
                fp.Close();
                return;
            }

            while (true)
            {                
                int ch = fp.Read();
                if (ch == -1) break;
                int c = CardHelper.char_to_card_num((char)ch);
                if (c < 0) continue;

                tmp.Add(c);
            }
            fp.Close();
            if (tmp.Count != 52)
            {
                lprintf($"invalid data.");
                return;
            }
        }

        List<Card> _pyramid = new List<Card>(28);
        List<Card> _deck = new List<Card>(24);

        // 0+3 -> pyramid[25:]
        // 3+6 -> pyramid[19:]
        // 9+9 -> pyramid[10:]
        // 18+10 -> pyramid[0:]
        _pyramid.AddRange(tmp.GetRange(18, 10));
        _pyramid.AddRange(tmp.GetRange(9, 9));
        _pyramid.AddRange(tmp.GetRange(3, 6));
        _pyramid.AddRange(tmp.GetRange(0, 3));
        // 28+24 -> deck
        _deck.AddRange(tmp.GetRange(28, tmp.Count - 28));

        lprintf("pyramid: ");
        foreach(var it in _pyramid)
        {
            printf($"{CardHelper.num_single[it]} ");
        }
        lprintf(string.Empty);

        lprintf("deck: ");
        foreach (var it in _deck)
        {
            printf($"{CardHelper.num_single[it]} ");
        }
        lprintf(string.Empty);

        if (_pyramid.Count != 28 || _deck.Count != 24)
        {
            lprintf($"invalid data length ({(int)_pyramid.Count}, {(int)_deck.Count}");
            return;
        }

        int[] cnt = new int[13];
        for (int i = 0; i < 28; ++i) ++cnt[_pyramid[i]];
        for (int i = 0; i < 24; ++i) ++cnt[_deck[i]];
        for (int i = 0; i < 13; ++i)
        {
            if (cnt[i] != 4)
            {
                lprintf("$invalid data.");
                lprintf($"{cnt}");
                return;
            }
        }

        var tripeak = new TriPeaks();

        tripeak.deck = _deck;
        tripeak.pyramid = _pyramid;
        tripeak.init();

        lprintf("Go!!!!!!!");
        tripeak.stop.Start();
        tripeak.solve();
        tripeak.stop.Stop();
        tripeak.output_solution();
        return;
    }
}
// 8Q8AAKK0260739346Q96A227 437058A574K26JQQ439J05J89JK5
