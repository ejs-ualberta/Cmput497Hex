name = input("Enter filename: ")
n = int(name.split('_')[0])

midpoint = (n*n)//2 + n%2



file = open(name)
lines = file.readlines()
idx = 1

def increment(l):
    l1 = []
    for item in l:
        x = int(item)
        if (x >= midpoint):
            x += 1
        l1.append(str(x))
    return l1
    

def next_line():
    global idx
    if idx >= len(lines):
        return
    while lines[idx].strip() == '':
        idx += 1;

def get_moves(line, i):
    return line.strip()[1:-2].split(' ')[i:]

output = [lines[0]]
next_line()
while idx < len(lines) and "RN 1" in lines[idx]:
    output.append(lines[idx])
    idx += 1
    next_line()
    # move past BN line
    output.append(lines[idx])
    idx += 1
    next_line()
    while "BN" in lines[idx]:
        output.append(lines[idx])
        idx += 1
        next_line()
        l = get_moves(lines[idx], 2)
        output.append('"WM ' + str(len(l)) + " " + " ".join(increment(l)) + '",')

        idx += 1
        next_line()
        l = get_moves(lines[idx], 1);
        output.append('"BM ' + " ".join(increment(l)) + '",')

        #ND
        idx += 1
        next_line()
        output.append(lines[idx]);

        #PS
        idx += 1
        next_line()
        if ("PS" in lines[idx]):
            output.append(lines[idx]);

        #PP
        idx += 1
        next_line()
        while ("BN" not in lines[idx] and "RN" not in lines[idx]):
            pp = lines[idx].strip().split(' ')[1]
            l = get_moves(lines[idx], 2);
            output.append('"PP ' + pp + ' ' + " ".join(increment(l)) + '",')
            idx += 1
            next_line()

while idx < len(lines):
    output.append(lines[idx])
    idx += 1
    next_line()

file.close()

file = open(name, 'w')
for line in output:
    file.write(line + '\n')

file.close()

