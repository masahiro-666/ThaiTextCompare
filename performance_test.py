#!/usr/bin/env python3
import subprocess
import time
import statistics

def run_performance_test():
    """Run performance tests on the Thai Text Compare system"""
    print("🚀 Thai Text Compare - Performance & Stress Test")
    print("=" * 50)
    
    # Test 1: Multiple runs timing
    print("\n📊 Test 1: Application Startup and Processing Time")
    times = []
    for i in range(5):
        start_time = time.time()
        result = subprocess.run(['dotnet', 'run', '--no-build'], 
                              cwd='/Users/masahiro/Library/Mobile Documents/com~apple~CloudDocs/BU/CoSI/Project-Synpaet/ThaiTextCompare',
                              capture_output=True, text=True)
        end_time = time.time()
        execution_time = end_time - start_time
        times.append(execution_time)
        print(f"  Run {i+1}: {execution_time:.2f}s")
    
    print(f"\n📈 Statistics:")
    print(f"  Average time: {statistics.mean(times):.2f}s")
    print(f"  Min time: {min(times):.2f}s")
    print(f"  Max time: {max(times):.2f}s")
    print(f"  Std deviation: {statistics.stdev(times):.2f}s")
    
    # Test 2: Unit test performance
    print(f"\n📊 Test 2: Unit Test Performance")
    test_times = []
    for i in range(3):
        start_time = time.time()
        result = subprocess.run(['dotnet', 'test', '--no-build'], 
                              cwd='/Users/masahiro/Library/Mobile Documents/com~apple~CloudDocs/BU/CoSI/Project-Synpaet/ThaiTextCompare',
                              capture_output=True, text=True)
        end_time = time.time()
        execution_time = end_time - start_time
        test_times.append(execution_time)
        print(f"  Test run {i+1}: {execution_time:.2f}s")
        
        # Extract test count from output
        if "Total tests:" in result.stdout:
            lines = result.stdout.split('\n')
            for line in lines:
                if "Total tests:" in line:
                    print(f"    {line.strip()}")
    
    print(f"\n📈 Test Statistics:")
    print(f"  Average test time: {statistics.mean(test_times):.2f}s")
    print(f"  64 tests per run")
    print(f"  Average time per test: {statistics.mean(test_times)/64*1000:.1f}ms")
    
    print(f"\n✅ Performance test completed!")

if __name__ == "__main__":
    run_performance_test()
