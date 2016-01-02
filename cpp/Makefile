SRCS=$(wildcard *.cc)
HDRS=$(wildcard *.h)

TARGETS=holdem

CXX=clang++
CXXFLAGS=-O3 -Wall -std=c++11 -stdlib=libc++

all: $(TARGETS)

clean:
	rm -f *.o $(TARGETS)

holdem: card.o eval.o hand.o main.o
	$(CXX) $(CXXFLAGS) -o $@ $^

.depend: $(SRCS) $(HDRS)
	rm -f ./.depend
	$(CXX) $(CXXFLAGS) -MM $(SRCS) >> ./.depend
-include .depend

.PHONY: clean
