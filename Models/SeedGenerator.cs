using Newtonsoft.Json;

namespace Models.Utilities.SeedGenerator
{
    #region exported types
    public interface ISeed<T>
    {
        //In order to separate from real and seeded instances
        public bool Seeded { get; set; }

        //Seeded The instance
        public T Seed(SeedGenerator seedGenerator);
    }

    public class SeededReviewBad
    {
        public string ReviewBad { get; init; }
    }
    public class SeededReviewAverage
    {
        public string ReviewAverage{ get; init; }
    }
    public class SeededReviewGood
    {
        public string ReviewGood { get; init; }
    }

    public class SeededTouristAttractionDescription
    {
        public string TouristAttractionDescription { get; init; }
    }
    #endregion

    public class SeedGenerator : Random
    {
        readonly SeedJsonContent _seeds = null;

        #region Names
        public string PetName => _seeds.Names.PetNames[this.Next(0, _seeds.Names.PetNames.Count)];
        public string FirstName => _seeds.Names.FirstNames[this.Next(0, _seeds.Names.FirstNames.Count)];
        public string LastName => _seeds.Names.LastNames[this.Next(0, _seeds.Names.LastNames.Count)];
        public string FullName => $"{FirstName} {LastName}";
        public string TouristAttractionName => _seeds.Names.TouristAttractionNames[this.Next(0, _seeds.Names.TouristAttractionNames.Count)];
        public string CategoryName => _seeds.Names.CategoryNames[this.Next(0, _seeds.Names.CategoryNames.Count)];

        #endregion

        #region Addresses
        public string Country => _seeds.Addresses[this.Next(0, _seeds.Addresses.Count)].Country;
        public string City(string Country = null)
        {
            if (Country != null)
            {
                var adr = _seeds.Addresses.FirstOrDefault(c => c.Country.ToLower() == Country.Trim().ToLower());
                if (adr == null)
                    throw new ArgumentException("Country not found");

                return adr.Cities[this.Next(0, adr.Cities.Count)];
            }

            var tmp = _seeds.Addresses[this.Next(0, _seeds.Addresses.Count)];
            return tmp.Cities[this.Next(0, tmp.Cities.Count)];
        }
        public string StreetAddress(string Country = null)
        {
            if (Country != null)
            {
                var adr = _seeds.Addresses.FirstOrDefault(c => c.Country.ToLower() == Country.Trim().ToLower());
                if (adr == null)
                    throw new ArgumentException("Country not found");

                return $"{adr.Streets[this.Next(0, adr.Streets.Count)]} {this.Next(1, 100)}";
            }

            var tmp = _seeds.Addresses[this.Next(0, _seeds.Addresses.Count)];
            return $"{tmp.Streets[this.Next(0, tmp.Streets.Count)]} {this.Next(1, 100)}";
        }
        public int ZipCode => this.Next(10101, 100000);
        #endregion

        #region Emails and phones
        public string Email(string fname = null, string lname = null)
        {
            fname ??= FirstName;
            lname ??= LastName;

            return $"{fname}.{lname}@{_seeds.Domains.Domains[this.Next(0, _seeds.Domains.Domains.Count)]}";
        }

        public string PhoneNr => $"{this.Next(700, 800)} {this.Next(100, 1000)} {this.Next(100, 1000)}";
        #endregion



        #region Reviews Bad
        public List<SeededReviewBad> AllReviewsBad => _seeds.ReviewsBad
            .Select(q => new SeededReviewBad { ReviewBad = q.ReviewBad })
            .ToList<SeededReviewBad>();

        public List<SeededReviewBad> ReviewsBad(int tryNrOfItems)
        {
            return UniqueIndexPickedFromList(tryNrOfItems, AllReviewsBad);
        }

        public SeededReviewBad ReviewBad => ReviewsBad(1).FirstOrDefault();

        #endregion

        #region Reviews Average
        public List<SeededReviewAverage> AllReviewsAverage => _seeds.ReviewsAverage
            .Select(q => new SeededReviewAverage { ReviewAverage = q.ReviewAverage })
            .ToList<SeededReviewAverage>();

        public List<SeededReviewAverage> ReviewsAverage(int tryNrOfItems)
        {
            return UniqueIndexPickedFromList(tryNrOfItems, AllReviewsAverage);
        }

        public SeededReviewAverage ReviewAverage => ReviewsAverage(1).FirstOrDefault();

        #endregion

        #region Reviews Good
        public List<SeededReviewGood> AllReviewsGood => _seeds.ReviewsGood
            .Select(q => new SeededReviewGood { ReviewGood = q.ReviewGood })
            .ToList<SeededReviewGood>();

        public List<SeededReviewGood> ReviewsGood(int tryNrOfItems)
        {
            return UniqueIndexPickedFromList(tryNrOfItems, AllReviewsGood);
        }

        public SeededReviewGood ReviewGood => ReviewsGood(1).FirstOrDefault();

        #endregion

        #region Tourist Attraction Description
        public List<SeededTouristAttractionDescription> AllTouristAttractionDescriptions => _seeds.TouristAttractionDescriptions
            .Select(q => new SeededTouristAttractionDescription { TouristAttractionDescription = q.TouristAttractionDescription })
            .ToList<SeededTouristAttractionDescription>();

        public List<SeededTouristAttractionDescription> TouristAttractionDescriptions(int tryNrOfItems)
        {
            return UniqueIndexPickedFromList(tryNrOfItems, AllTouristAttractionDescriptions);
        }

        public SeededTouristAttractionDescription TouristAttractionDescription => TouristAttractionDescriptions(1).FirstOrDefault();

        #endregion

        #region DateTime, bool and decimal
        public DateTime DateAndTime(int? fromYear = null, int? toYear = null)
        {
            bool dateOK = false;
            DateTime _date = default;
            while (!dateOK)
            {
                fromYear ??= DateTime.Today.Year;
                toYear ??= DateTime.Today.Year + 1;

                try
                {
                    int year = this.Next(Math.Min(fromYear.Value, toYear.Value),
                        Math.Max(fromYear.Value, toYear.Value));
                    int month = this.Next(1, 13);
                    //int day = this.Next(1, 32);
                    int day = this.Next(1, DateTime.DaysInMonth(year, month) + 1);

                    _date = new DateTime(year, month, day);

                    dateOK = true;
                }
                catch
                {
                    dateOK = false;
                }
            }

            return DateTime.SpecifyKind(_date, DateTimeKind.Utc);
        }

        public bool Bool => (this.Next(0, 10) < 5) ? true : false;

        public decimal NextDecimal(int _from, int _to) => this.Next(_from * 1000, _to * 1000) / 1000M;
        #endregion

        #region From own String, Enum and List<TItem>
        public string FromString(string _inputString, string _splitDelimiter = ", ")
        {
            var _sarray = _inputString.Split(_splitDelimiter);
            return _sarray[this.Next(0, _sarray.Length)];
        }
        public TEnum FromEnum<TEnum>() where TEnum : struct
        {
            if (typeof(TEnum).IsEnum)
            {

                var _names = typeof(TEnum).GetEnumNames();
                var _name = _names[this.Next(0, _names.Length)];

                return Enum.Parse<TEnum>(_name);
            }
            throw new ArgumentException("Not an enum type");
        }
        public TItem FromList<TItem>(List<TItem> items)
        {
            return items[this.Next(0, items.Count)];
        }
        #endregion

        #region Generate seeded List of TItem

        //ISeed<TItem> has to be implemented to use this method
        public List<TItem> ItemsToList<TItem>(int NrOfItems)
            where TItem : ISeed<TItem>, new()
        {
            //Create a list of seeded items
            var _list = new List<TItem>();
            for (int c = 0; c < NrOfItems; c++)
            {
                _list.Add(new TItem() { Seeded = true }.Seed(this));
            }
            return _list;
        }

        //Create a list of unique randomly seeded items
        public List<TItem> UniqueItemsToList<TItem>(int tryNrOfItems, List<TItem> appendToUnique = null)
            where TItem : ISeed<TItem>, IEquatable<TItem>, new()
        {
            //Create a list of uniquely seeded items
            HashSet<TItem> _set = (appendToUnique == null) ? new HashSet<TItem>() : new HashSet<TItem>(appendToUnique);

            while (_set.Count < tryNrOfItems)
            {
                var _item = new TItem() { Seeded = true }.Seed(this);

                int _preCount = _set.Count;
                int tries = 0;
                do
                {
                    _set.Add(_item);

                    if (_set.Count == _preCount)
                    {
                        //Item was already in the _set. Generate a new one
                        _item = new TItem() { Seeded = true }.Seed(this);
                        ++tries;

                        //Does not seem to be able to generate new unique item
                        if (tries > 5)
                            return _set.ToList();
                    }

                } while (_set.Count <= _preCount);
            }

            return _set.ToList();
        }

        //Pick a number of unique items from a list of TItem (the List does not have to be unique)
        //IEquatable<TItem> has to be implemented to use this method
        public List<TItem> UniqueItemsPickedFromList<TItem>(int tryNrOfItems, List<TItem> list)
        where TItem : IEquatable<TItem>
        {
            //Create a list of uniquely seeded items
            HashSet<TItem> _set = new HashSet<TItem>();

            while (_set.Count < tryNrOfItems)
            {
                var _item = list[this.Next(0, list.Count)];

                int _preCount = _set.Count;
                int tries = 0;
                do
                {
                    _set.Add(_item);

                    if (_set.Count == _preCount)
                    {
                        //Item was already in the _set. Pick a new one
                        _item = list[this.Next(0, list.Count)];
                        ++tries;

                        //Does not seem to be able to pick new unique item
                        if (tries > 5)
                            return _set.ToList();
                    }

                } while (_set.Count <= _preCount);
            }

            return _set.ToList();
        }

        //Pick a number of items, all with unique indexes, from a list of TItem
        public List<TItem> UniqueIndexPickedFromList<TItem>(int tryNrOfItems, List<TItem> list)
                where TItem : new()
        {
            //Create a hashed list of unique indexes
            HashSet<int> _set = new HashSet<int>();

            while (_set.Count < tryNrOfItems)
            {
                var _idx = this.Next(0, list.Count);

                int _preCount = _set.Count;
                int tries = 0;
                do
                {
                    _set.Add(_idx);

                    if (_set.Count == _preCount)
                    {
                        //Idx was already in the _set. Generate a new one
                        _idx = this.Next(0, list.Count);
                        ++tries;

                        //Does not seem to be able to generate new unique idx
                        if (tries > 5)
                            break;
                    }

                } while (_set.Count <= _preCount);
            }

            //I have now a set of unique idx
            //return a list of items from a list with indexes
            var retList = new List<TItem>();
            foreach (var item in _set)
            {
                retList.Add(list[item]);
            }
            return retList;
        }
        #endregion

        #region initialize master content
        SeedJsonContent CreateMasterSeedFile()
        {
            return new SeedJsonContent()
            {

                ReviewsBad = new List<SeedReviewBad>
{
    new SeedReviewBad { jsonReviewBad = "Overcrowded and noisy, couldn’t enjoy the view at all." },
    new SeedReviewBad { jsonReviewBad = "Way too expensive for what it offers, very disappointing." },
    new SeedReviewBad { jsonReviewBad = "Dirty surroundings and poorly maintained facilities." },
    new SeedReviewBad { jsonReviewBad = "Staff was rude and unhelpful when asked for directions." },
    new SeedReviewBad { jsonReviewBad = "Long waiting lines ruined the whole experience." },
    new SeedReviewBad { jsonReviewBad = "Looked nothing like the pictures online, very misleading." },
    new SeedReviewBad { jsonReviewBad = "Food options nearby were terrible and overpriced." },
    new SeedReviewBad { jsonReviewBad = "The attraction was closed without any prior notice." },
    new SeedReviewBad { jsonReviewBad = "Unsafe area, felt uncomfortable walking around." },
    new SeedReviewBad { jsonReviewBad = "Guided tour was boring and uninformative." },
    new SeedReviewBad { jsonReviewBad = "Not worth the time, I regret visiting." },
    new SeedReviewBad { jsonReviewBad = "Very small and underwhelming compared to expectations." },
    new SeedReviewBad { jsonReviewBad = "Waited over an hour just to get in, and it really wasn’t worth it." },
new SeedReviewBad { jsonReviewBad = "Everything felt overpriced, even the bottled water." },
new SeedReviewBad { jsonReviewBad = "Crowds everywhere, couldn’t enjoy a single moment." },
new SeedReviewBad { jsonReviewBad = "Looked better online than in real life, very underwhelming." },
new SeedReviewBad { jsonReviewBad = "Bathrooms were dirty and hard to find." },
new SeedReviewBad { jsonReviewBad = "Tour guide sounded bored and rushed through everything." },

},
                ReviewsAverage = new List<SeedReviewAverage>
{
    new SeedReviewAverage { jsonReviewAverage = "It was okay, but nothing particularly special." },
    new SeedReviewAverage { jsonReviewAverage = "The view was decent, but crowded most of the time." },
    new SeedReviewAverage { jsonReviewAverage = "Average experience, expected a bit more." },
    new SeedReviewAverage { jsonReviewAverage = "Good to see once, but I wouldn’t go back." },
    new SeedReviewAverage { jsonReviewAverage = "Nice atmosphere, but overpriced tickets." },
    new SeedReviewAverage { jsonReviewAverage = "Interesting, but not as memorable as I hoped." },
    new SeedReviewAverage { jsonReviewAverage = "Facilities were fine, though nothing stood out." },
    new SeedReviewAverage { jsonReviewAverage = "The place was okay, but service could be better." },
    new SeedReviewAverage { jsonReviewAverage = "Worth a short stop, but don’t expect too much." },
    new SeedReviewAverage { jsonReviewAverage = "A fair experience, not too bad, not too great." },
    new SeedReviewAverage { jsonReviewAverage = "Good for a quick visit, not for a whole day." },
    new SeedReviewAverage { jsonReviewAverage = "Enjoyable, but I’ve seen better attractions elsewhere." },
    new SeedReviewAverage { jsonReviewAverage = "The place was nice, but I wouldn’t go out of my way to see it again." },
new SeedReviewAverage { jsonReviewAverage = "Some great photo spots, but also a lot of construction going on." },
new SeedReviewAverage { jsonReviewAverage = "Decent experience, nothing spectacular though." },
new SeedReviewAverage { jsonReviewAverage = "Fun for a quick stop, but not worth a full day." },
new SeedReviewAverage { jsonReviewAverage = "Good atmosphere, but a bit too crowded for my taste." },
new SeedReviewAverage { jsonReviewAverage = "I liked it, but I’ve seen more impressive places elsewhere." },

},
                ReviewsGood = new List<SeedReviewGood>
{
    new SeedReviewGood { jsonReviewGood = "Absolutely stunning! A must-see attraction." },
    new SeedReviewGood { jsonReviewGood = "Breathtaking views and a wonderful atmosphere." },
    new SeedReviewGood { jsonReviewGood = "One of the best experiences of my trip." },
    new SeedReviewGood { jsonReviewGood = "Highly recommend visiting, worth every penny." },
    new SeedReviewGood { jsonReviewGood = "Beautifully maintained and very enjoyable." },
    new SeedReviewGood { jsonReviewGood = "The guided tour was fun, informative, and engaging." },
    new SeedReviewGood { jsonReviewGood = "Great photo opportunities and lovely surroundings." },
    new SeedReviewGood { jsonReviewGood = "Exceeded my expectations in every way." },
    new SeedReviewGood { jsonReviewGood = "Perfect for families and travelers alike." },
    new SeedReviewGood { jsonReviewGood = "The highlight of my vacation, unforgettable." },
    new SeedReviewGood { jsonReviewGood = "Friendly staff and very well organized." },
    new SeedReviewGood { jsonReviewGood = "Amazing cultural experience, I would visit again." },
    new SeedReviewGood { jsonReviewGood = "Absolutely loved it! The views were incredible." },
new SeedReviewGood { jsonReviewGood = "Such a unique experience, would definitely recommend." },
new SeedReviewGood { jsonReviewGood = "Worth every minute, especially around sunset." },
new SeedReviewGood { jsonReviewGood = "Great vibes, friendly staff, and lots to see." },
new SeedReviewGood { jsonReviewGood = "Perfect spot for photos and making memories." },
new SeedReviewGood { jsonReviewGood = "Exceeded my expectations, I’d happily come back again." }

},
                TouristAttractionDescriptions = new List<SeedTouristAttractionDescription>
{
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A historic landmark known for its cultural significance and stunning architecture." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A natural wonder attracting visitors from all over the world." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Famous for its breathtaking views and unique surroundings." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A popular destination featuring beautiful gardens and walking paths." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Known for its rich history and fascinating stories from the past." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A vibrant attraction filled with art, music, and cultural events." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A peaceful retreat offering scenic landscapes and fresh air." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A must-see spot with iconic views and unforgettable experiences." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "An impressive structure showcasing architectural brilliance." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A lively area known for shopping, dining, and entertainment." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A charming old town full of narrow streets and historic buildings." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Famous for its panoramic viewpoints and sunset scenes." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A beautiful blend of history, culture, and modern attractions." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Renowned for its stunning artwork and priceless collections." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A landmark offering guided tours and educational experiences." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Surrounded by nature, it is a favorite spot for outdoor activities." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A bustling square filled with life, history, and energy." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A coastal destination with sandy beaches and clear waters." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "An awe-inspiring monument that leaves visitors speechless." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A peaceful lake area ideal for boating and picnics." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A spectacular viewpoint overlooking the city skyline." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Famous for its vibrant markets and lively atmosphere." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A quiet park with shaded paths and relaxing green spaces." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A historical museum preserving artifacts and ancient treasures." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "An architectural masterpiece admired worldwide." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A sacred site attracting pilgrims and tourists alike." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A dramatic mountain range popular for hiking and skiing." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A UNESCO World Heritage Site filled with cultural importance." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "An iconic tower recognized as a symbol of the region." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A beautiful cathedral showcasing centuries of craftsmanship." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A famous palace with lavish interiors and sprawling gardens." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A modern landmark representing innovation and design." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A lively festival area hosting seasonal events and performances." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A picturesque village with traditional houses and local charm." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A grand plaza surrounded by historic monuments and cafes." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A peaceful island escape known for its natural beauty." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A dramatic canyon carved by centuries of natural forces." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A fascinating archaeological site that reveals ancient civilizations." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A scenic coastal walkway perfect for sunsets and photos." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A lively entertainment district offering nightlife and dining." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A botanical garden with rare plants and exotic flowers." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "An old fortress standing as a reminder of past battles." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A historic bridge admired for its design and engineering." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A traditional market filled with spices, crafts, and souvenirs." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A mountain peak known for its hiking trails and panoramic views." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A coastal harbor offering boat tours and seafood restaurants." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "An artistic neighborhood full of murals and creative spaces." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A natural park with wildlife, trails, and peaceful scenery." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A cultural center hosting exhibitions, concerts, and events." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A perfect spot to take a stroll and snap some great photos." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Loved wandering around, lots of hidden corners to explore." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Great for families, with plenty of space for kids to play." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Awesome views and a really chill vibe, would visit again." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Packed with history but easy to enjoy even without a guide." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Perfect for a sunny afternoon, lots of cafes nearby." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A cool place to relax and soak up the local atmosphere." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Fun experience, lots of photo opportunities everywhere." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Great for a quick stop, but you could easily spend half a day here." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Loved the vibe, felt like a hidden gem in the city." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Easy to get to, and a lot more interesting than I expected." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Lots of local flavor, perfect for trying traditional snacks." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A really relaxed place to wander, not too crowded." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Perfect spot to watch the sunset and chill." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Felt like stepping back in time, really atmospheric." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Nice little corners to explore, fun for wandering around." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Great for casual photography, loved the colors and textures." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Loved the energy, felt alive but still manageable crowds." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Good mix of shops, food, and local charm." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A short walk but packed with interesting sights." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Cool local spot, perfect for people watching and relaxing." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Easy to navigate, with lots of little surprises along the way." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A friendly, vibrant place to spend a morning or afternoon." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Loved discovering the small details and local touches." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Perfect for a casual day out, lots of benches and greenery." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A lively area with music, street performers, and good energy." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Fun place to just wander and see what you stumble upon." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Great mix of old and new, really interesting architecture." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "A hidden corner that feels peaceful despite the city buzz." },
    new SeedTouristAttractionDescription { jsonTouristAttractionDescription = "Perfect for grabbing a coffee and watching the world go by." }

},

                Addresses = new List<SeedAddress>
                {
                        new SeedAddress {
    jsonCountry = "Sweden",
    jsonCities = "Stockholm, Göteborg, Malmö, Uppsala, Linköping, Örebro, Västerås, Helsingborg, Jönköping, Visby, Kiruna, Sundsvall, Umeå",
    jsonStreets = "Svedjevägen, Ringvägen, Vasagatan, Odenplan, Birger Jarlsgatan, Äppelviksvägen, Kvarnbacksvägen, Drottninggatan, Storgatan, Sveavägen, Hornsgatan, Linnégatan, Folkungagatan, Odengatan, Nybrogatan, Karlavägen, Götgatan, Kungsgatan, Regeringsgatan, Sankt Eriksgatan"
},
new SeedAddress {
    jsonCountry = "Norway",
    jsonCities = "Oslo, Bergen, Trondheim, Stavanger, Drammen, Tromsø, Kristiansand, Bodø, Ålesund, Lillehammer, Lofoten, Hammerfest, Haugesund, Sandefjord, Arendal, Svalbard",
    jsonStreets = "Bygdøy allé, Frognerveien, Pilestredet, Vidars gate, Sågveien, Toftes gate, Gardeveien, Kirkeveien, Ullevålsveien, Bogstadveien, Karl Johans gate, Storgata, Grønlandsleiret, Skippergata, Kongens gate, Holbergs gate, Parkveien, Schweigaards gate, Tollbugata, Torggata"
},
new SeedAddress {
    jsonCountry = "Denmark",
    jsonCities = "Köpenhamn, Århus, Odense, Aalborg, Esbjerg, Roskilde, Helsingør, Randers, Skagen, Vejle, Bornholm, Sønderborg, Silkeborg, Horsens, Herning, Kolding, Hillerød",
    jsonStreets = "Rolighedsvej, Fensmarkgade, Svanevej, Grøn­dalsvej, Githersgade, Classensgade, Moltekesvej, Vesterbrogade, Nørrebrogade, Østerbrogade, Amagerbrogade, Gammel Kongevej, Frederiksberg Allé, Falkoner Allé, Sølvgade, Bredgade, Nyhavn, Strandgade, Borgergade, Store Kongensgade"
},
new SeedAddress {
    jsonCountry = "Finland",
    jsonCities = "Helsingfors, Espoo, Tampere, Vantaa, Oulu, Turku, Jyväskylä, Lahti, Kuopio, Rovaniemi, Kemi, Lappland, Åbo, Kouvola, Pori, Joensuu",
    jsonStreets = "Arkadiankatu, Liisankatu, Ruoholahdenkatu, Pohjoistranta, Eerikinkatu, Vauhtitie, Itäinen Vaideki, Mannerheimintie, Aleksanterinkatu, Kaisaniemenkatu, Kaivokatu, Kalevankatu, Lönnrotinkatu, Hietalahdenkatu, Töölönkatu, Runeberginkatu, Fredrikinkatu, Tehtaankatu, Korkeavuorenkatu, Iso Roobertinkatu"
},
new SeedAddress {
    jsonCountry = "Germany",
    jsonCities = "Berlin, Hamburg, Munich, Cologne, Frankfurt, Stuttgart, Düsseldorf, Dortmund, Bremen, Dresden, Leipzig, Hanover, Nuremberg, Black Forest",
    jsonStreets = "Unter den Linden, Kurfürstendamm, Friedrichstrasse, Karl-Marx-Allee, Schönhauser Allee, Leipziger Strasse, Oranienburger Strasse, Müllerstrasse, Sonnenallee, Tauentzienstrasse, Frankfurter Allee, Wilhelmstrasse, Brunnenstrasse, Potsdamer Strasse, Hardenbergstrasse, Kantstrasse, Karlstrasse, Maximilianstrasse, Theatinerstrasse, Sendlinger Strasse"
},
new SeedAddress {
    jsonCountry = "France",
    jsonCities = "Paris, Marseille, Lyon, Toulouse, Nice, Nantes, Strasbourg, Montpellier, Bordeaux, Lille, Rennes, Provence, Corsica, Reims, Dijon",
    jsonStreets = "Champs-Élysées, Rue de Rivoli, Boulevard Haussmann, Avenue Montaigne, Rue Saint-Honoré, Rue de la Paix, Boulevard Saint-Michel, Rue Mouffetard, Rue de Rennes, Rue de Vaugirard, Rue de la République, Rue Victor Hugo, Rue Paradis, Rue d’Antibes, Rue Foch, Cours Mirabeau, Rue Nationale, Rue Alsace-Lorraine, Rue Sainte-Catherine, Avenue Jean Médecin"
},
new SeedAddress {
    jsonCountry = "Italy",
    jsonCities = "Rome, Milan, Naples, Turin, Palermo, Genoa, Bologna, Florence, Bari, Catania, Venice, Verona, Pisa, Amalfi Coast",
    jsonStreets = "Via del Corso, Via Condotti, Via Veneto, Via Nazionale, Via Appia, Via dei Fori Imperiali, Via Cavour, Corso Vittorio Emanuele, Via Toledo, Via Roma, Via Garibaldi, Via Dante, Corso Italia, Via Mazzini, Via Manzoni, Via XX Settembre, Corso Buenos Aires, Corso Porta Nuova, Via Ricasoli, Via Tornabuoni"
},
new SeedAddress {
    jsonCountry = "Spain",
    jsonCities = "Madrid, Barcelona, Valencia, Seville, Zaragoza, Málaga, Murcia, Bilbao, Alicante, Córdoba, Granada, Salamanca, Mallorca, Valladolid",
    jsonStreets = "Gran Vía, Paseo de la Castellana, Calle de Alcalá, Calle Mayor, Calle Serrano, Calle Princesa, Calle de Atocha, Paseo del Prado, Rambla de Catalunya, Passeig de Gràcia, Avinguda Diagonal, Carrer de Balmes, Calle Larios, Calle Nueva, Avenida de la Constitución, Calle San Fernando, Calle Feria, Paseo Marítimo, Calle Colón, Gran Vía de Colón"
},
new SeedAddress {
    jsonCountry = "United Kingdom",
    jsonCities = "London, Birmingham, Manchester, Liverpool, Leeds, Sheffield, Bristol, Newcastle, Glasgow, Edinburgh, Cardiff, Belfast, Nottingham, York, Lake District",
    jsonStreets = "Oxford Street, Regent Street, Baker Street, Fleet Street, Strand, Charing Cross Road, Kingsway, Whitehall, Pall Mall, Piccadilly, Bond Street, Tottenham Court Road, Kensington High Street, Victoria Street, Shaftesbury Avenue, Euston Road, Park Lane, Abbey Road, Brick Lane, Camden High Street"
},
new SeedAddress {
    jsonCountry = "United States",
    jsonCities = "New York, Los Angeles, Chicago, Houston, Phoenix, Philadelphia, San Antonio, San Diego, Dallas, San Jose, Miami, Boston, Las Vegas, Yellowstone, Austin, Jacksonville, San Francisco",
    jsonStreets = "Broadway, Wall Street, Fifth Avenue, Madison Avenue, Park Avenue, Lexington Avenue, Times Square, Rodeo Drive, Sunset Boulevard, Hollywood Boulevard, Lombard Street, Market Street, Michigan Avenue, State Street, Beale Street, Bourbon Street, Canal Street, Pennsylvania Avenue, Constitution Avenue, K Street"
},

                },
                Names = new SeedNames
                {
                    jsonFirstNames = "Harry, Lord, Hermione, Albus, Severus, Ron, Draco, Frodo, Gandalf, Sam, Peregrin, Saruman, Alex, Steve, Maria, John, Sarah, David, Emily, James, Linda, Michael, Robert, Jessica, Daniel, Karen, Mark, Lisa, Paul, Angela, Brian, Susan",
                    jsonLastNames = "Potter, Voldemort, Granger, Dumbledore, Snape, Malfoy, Baggins, the Gray, Gamgee, Took, the White",
                    jsonPetNames = "Max, Charlie, Cooper, Milo, Rocky, Wanda, Teddy, Duke, Leo, Max, Simba, Smith, Johnson, Williams, Brown, Jones, Garcia, Miller, Davis, Rodriguez, Wilson, Martinez, Anderson, Taylor, Thomas, Hernandez, Moore, Martin, Jackson, Thompson, White",
                    jsonTouristAttractionNames = "Eiffel Tower, Colosseum, Statue of Liberty, Great Wall of China, Machu Picchu, Taj Mahal, Christ the Redeemer, Big Ben, Louvre Museum, Sydney Opera House, Golden Gate Bridge, Mount Fuji, Burj Khalifa, Sagrada Familia, Petra, Acropolis of Athens, Angkor Wat, Mount Everest, Niagara Falls, Stonehenge, Times Square, Mount Kilimanjaro, Buckingham Palace, Grand Canyon, Leaning Tower of Pisa, Vatican City, Mount Rushmore, Alhambra, Tower Bridge, Yellowstone National Park, Hollywood Sign, Chichen Itza, Matterhorn, Kremlin, Saint Basil’s Cathedral, Brandenburg Gate, Palace of Versailles, Red Square, Mount Vesuvius, Yosemite National Park, Mount Etna, Great Barrier Reef, Westminster Abbey, Edinburgh Castle, Table Mountain, CN Tower, Berlin Wall, Neuschwanstein Castle, Central Park, Dubai Mall, Blue Mosque, Shwedagon Pagoda, Forbidden City, Mount Olympus, Pantheon, Piazza San Marco, Rialto Bridge, Dubai Fountain, Hagia Sophia, Cappadocia, Mecca Kaaba, Mount Sinai, Dead Sea, Dome of the Rock, Western Wall, Mount Ararat, Iguazu Falls, Galapagos Islands, Serengeti National Park, Victoria Falls, Mount McKinley, Mount Denali, Rocky Mountains, Banff National Park, Lake Louise, Yosemite Falls, Sequoia National Park, Mount Rainier, Mount St. Helens, Death Valley, Zion National Park, Bryce Canyon, Arches National Park, Antelope Canyon, Monument Valley, Mesa Verde, Hoover Dam, Mount Hood, Crater Lake, Grand Teton, Mount Whitney, Joshua Tree, White Sands, Carlsbad Caverns, Alcatraz Island, Mount Shasta, Redwood National Park, Mount Washington, Acadia National Park, Great Smoky Mountains, Everglades, Key West, Kennedy Space Center, Mount Haleakala, Diamond Head, Pearl Harbor, Waimea Canyon, Bora Bora Lagoon, Mount Cook, Milford Sound, Tongariro National Park, Fiordland, Rotorua Geysers, Hobbiton, Great Ocean Road, Uluru, Kakadu National Park, Fraser Island, Bondi Beach, Daintree Rainforest, Sydney Harbour Bridge, Twelve Apostles, Mount Kosciuszko, Melbourne Royal Botanic Gardens, Taronga Zoo, Singapore Marina Bay Sands, Gardens by the Bay, Merlion, Sentosa Island, Universal Studios Singapore, Orchard Road, Singapore Flyer, Esplanade Theatre, Mount Kinabalu, Borneo Rainforest, Komodo National Park, Borobudur Temple, Prambanan Temple, Bali Rice Terraces, Tanah Lot, Ubud Monkey Forest, Mount Bromo, Raja Ampat Islands, Toba Lake, Gili Islands, Shinjuku Gyoen, Tokyo Tower, Tokyo Disneyland, Tokyo Skytree, Shibuya Crossing, Fushimi Inari Shrine, Kinkaku-ji, Osaka Castle, Nara Park, Itsukushima Shrine, Hiroshima Peace Memorial, Himeji Castle, Okinawa Beaches, Mount Koya, Nikko Shrines, Sapporo Snow Festival, Seoul Tower, Gyeongbokgung Palace, Changdeokgung, Bukchon Hanok Village, Jeju Island, DMZ Korea, Lotte World, Everland, Mount Hallasan, Bukhansan National Park, Hong Kong Disneyland, Victoria Peak, Tian Tan Buddha, Star Ferry, Temple Street Market, Ocean Park Hong Kong, Macau Tower, Ruins of St. Paul’s, Venetian Macau, Senado Square, Mount Huangshan, Terracotta Army, Li River, Zhangjiajie National Forest, West Lake, Potala Palace, Mount Kailash, Mount Emei, Leshan Giant Buddha, Jiuzhaigou Valley, Silk Road, Mount Everest Base Camp, Mount K2, Karakoram Highway, Lahore Fort, Badshahi Mosque, Hunza Valley, Mohenjo-daro, Faisal Mosque, Mount Annapurna, Kathmandu Durbar Square, Bhaktapur Durbar Square, Swayambhunath Stupa, Lumbini, Mount Everest Base Camp Nepal, Bhutan Tiger’s Nest, Thimphu, Punakha Dzong, Paro Valley",
                    jsonCategoryNames = "Historical Sites, Museums, Art Galleries, Castles, Palaces, Ancient Ruins, Temples, Churches, Cathedrals, Mosques, Monasteries, Archaeological Sites, Cultural Villages, Theatres, Opera Houses, Landmarks, Monuments, Statues, Bridges, Towers, Natural Wonders, Mountains, Volcanoes, Caves, Waterfalls, Lakes, Rivers, Beaches, Islands, National Parks, Wildlife Reserves, Botanical Gardens, Zoos, Aquariums, Theme Parks, Amusement Parks, Markets, Shopping Districts, Festivals, Heritage Trails, Scenic Roads",
                },
                Domains = new SeedDomains
                {
                    jsonDomainNames = "icloud.com, me.com, mac.com, hotmail.com, gmail.com, protonmail.com, outlook.com, yahoo.com, mail.com, aol.com"
                }

            };
        }
        #endregion

        #region create master json file
        public string WriteMasterStream()
        {
            return CreateMasterSeedFile().WriteFile("app-seeds.json");
        }

        // Write master seed JSON directly to the provided file path
        public string WriteMasterStream(string filePath)
        {
            var seeds = CreateMasterSeedFile();

            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.GetFullPath(filePath);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (Stream s = File.Create(fullPath))
            using (TextWriter writer = new StreamWriter(s))
            {
                writer.Write(JsonConvert.SerializeObject(seeds, Formatting.Indented));
            }

            return fullPath;
        }
        #endregion

        #region contructors
        public SeedGenerator()
        {
            _seeds = CreateMasterSeedFile();
        }
        public SeedGenerator(string SeedPathName)
        {
            if (!SeedJsonContent.FileExists(SeedPathName))
            {
                throw new FileNotFoundException(SeedPathName);
            }
            _seeds = SeedJsonContent.ReadFile(SeedPathName);
        }
        #endregion

        #region internal classes

        class SeedReviewBad
        {
            #region Bad reviews towards json file
            string _jsonReviewBad;
            public string jsonReviewBad { get => _jsonReviewBad; set => _jsonReviewBad = value; }

            #endregion

            [JsonIgnore]
            public string ReviewBad => _jsonReviewBad;

        }
        class SeedReviewAverage
        {
            #region Average reviews towards json file
            string _jsonReviewAverage;
            public string jsonReviewAverage { get => _jsonReviewAverage; set => _jsonReviewAverage = value; }

            #endregion

            [JsonIgnore]
            public string ReviewAverage => _jsonReviewAverage;

        }
        class SeedReviewGood
        {
            #region Good reviews towards json file
            string _jsonReviewGood;
            public string jsonReviewGood { get => _jsonReviewGood; set => _jsonReviewGood = value; }

            #endregion

            [JsonIgnore]
            public string ReviewGood => _jsonReviewGood;

        }
        class SeedTouristAttractionDescription
        {
            #region Tourist attraction descriptions towards json file
            string _jsonTouristAttractionDescription;
            public string jsonTouristAttractionDescription { get => _jsonTouristAttractionDescription; set => _jsonTouristAttractionDescription = value; }

            #endregion

            [JsonIgnore]
            public string TouristAttractionDescription => _jsonTouristAttractionDescription;

        }
        class SeedAddress
        {
            #region Country towards json file
            string _jsonCountry;
            public string jsonCountry { get => _jsonCountry; set { _jsonCountry = value; } }
            #endregion

            [JsonIgnore]
            public string Country => _jsonCountry;

            #region Streets towards json file
            string _jsonStreets;
            public string jsonStreets
            {
                get => _jsonStreets;
                set
                {
                    _jsonStreets = value;
                    _streets = _jsonStreets.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _streets;
            [JsonIgnore]
            public List<string> Streets => _streets;

            #region Cities towards json file
            string _jsonCities;
            public string jsonCities
            {
                get => _jsonCities;
                set
                {
                    _jsonCities = value;
                    _cities = _jsonCities.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _cities;
            [JsonIgnore]
            public List<string> Cities => _cities;
        }
        class SeedNames
        {
            #region Names towards json file
            string _jsonFirstNames;
            public string jsonFirstNames
            {
                get => _jsonFirstNames;
                set
                {
                    _jsonFirstNames = value;
                    _firstNames = _jsonFirstNames.Split(", ").ToList();
                }
            }

            string _jsonLastNames;
            public string jsonLastNames
            {
                get => _jsonLastNames;
                set
                {
                    _jsonLastNames = value;
                    _lastNames = _jsonLastNames.Split(", ").ToList();
                }
            }

            string _jsonPetNames;
            public string jsonPetNames
            {
                get => _jsonPetNames;
                set
                {
                    _jsonPetNames = value;
                    _petNames = _jsonPetNames.Split(", ").ToList();
                }
            }
            string _jsonTouristAttractionNames;
            public string jsonTouristAttractionNames
            {
                get => _jsonTouristAttractionNames;
                set
                {
                    _jsonTouristAttractionNames = value;
                    _touristAttractionNames = _jsonTouristAttractionNames.Split(", ").ToList();
                }
            }

            string _jsonCategoryNames;
            public string jsonCategoryNames
            {
                get => _jsonCategoryNames;
                set
                {
                    _jsonCategoryNames = value;
                    _categoryNames = _jsonCategoryNames.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _firstNames;
            [JsonIgnore]
            public List<string> FirstNames => _firstNames;

            List<string> _lastNames;
            [JsonIgnore]
            public List<string> LastNames => _lastNames;

            List<string> _petNames;
            [JsonIgnore]
            public List<string> PetNames => _petNames;

            List<string> _touristAttractionNames;
            [JsonIgnore]
            public List<string> TouristAttractionNames => _touristAttractionNames;

            List<string> _categoryNames;
            [JsonIgnore]
            public List<string> CategoryNames => _categoryNames;
            // List<string> _TouristAttractionDescription;
            // [JsonIgnore]
            // public List<string> TouristAttractionDescriptions => _TouristAttractionDescription;
        }
        class SeedDomains
        {
            #region Domains towards json file
            string _jsonDomainNames;
            public string jsonDomainNames
            {
                get => _jsonDomainNames;
                set
                {
                    _jsonDomainNames = value;
                    _domainNames = _jsonDomainNames.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _domainNames;
            [JsonIgnore]
            public List<string> Domains => _domainNames;
        }

        class SeedJsonContent
        {
            public List<SeedAddress> Addresses { get; set; } = new List<SeedAddress>();
            public List<SeedReviewBad> ReviewsBad { get; set; } = new List<SeedReviewBad>();
            public List<SeedReviewAverage> ReviewsAverage { get; set; } = new List<SeedReviewAverage>();
            public List<SeedReviewGood> ReviewsGood { get; set; } = new List<SeedReviewGood>();
            public List<SeedTouristAttractionDescription> TouristAttractionDescriptions { get; set; } = new List<SeedTouristAttractionDescription>();


            public SeedNames Names { get; set; } = new SeedNames();
            public SeedDomains Domains { get; set; } = new SeedDomains();

            public string WriteFile(string FileName) => WriteFile(this, FileName);
            public static string WriteFile(SeedJsonContent Seeds, string FileName)
            {
                var fn = fname(FileName);
                using (Stream s = File.Create(fn))
                using (TextWriter writer = new StreamWriter(s))
                {
                    writer.Write(JsonConvert.SerializeObject(Seeds, Formatting.Indented));
                }

                return fn;
            }

            public static SeedJsonContent ReadFile(string PathName)
            {
                SeedJsonContent seeds = null;
                using (Stream s = File.OpenRead(PathName))
                using (TextReader reader = new StreamReader(s))

                    seeds = JsonConvert.DeserializeObject<SeedJsonContent>(reader.ReadToEnd());

                return seeds;
            }

            static string fname(string name)
            {
                var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                documentPath = Path.Combine(documentPath, "SeedGenerator");
                if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
                return Path.Combine(documentPath, name);
            }

            public static bool FileExists(string FileName)
            {

                var fn = Path.GetFileName(FileName);
                if (fn == FileName)
                {
                    //no path in FileName use default directory
                    return File.Exists(fname(FileName));
                }

                return File.Exists(FileName);
            }
        }
        #endregion
    }
}

